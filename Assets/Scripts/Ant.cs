using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Notice the collider is a sphere and not a cone or something.
//find other resources in view in case the cur one was moved / destroyed
//pheromones should be under the same parent object
public class Ant : Gatherer
{
    public enum PheromoneSpredRate { Slow=8, Normal=5, Rapid=2};
    public static readonly float Pheromone_Spread_Default_Rate = 0.1f;
    public PheromoneSpredRate myPheromoneSpredRate;

    public static readonly float Max_Value_Direction = 1f;
    public static readonly float Min_Value_Direction = -1f;
    public static readonly float Max_Interval_Direction = .5f;
    public static readonly float Min_Interval_Direction = -.5f;

    GameObject detectionRange;

    public bool isScouting;
    public float minRandomTime;
    public float maxRandomTime;
    public float startTime;
    public float timeInDirection;
    public Quaternion quaternionTarget;

    public GameObject pheromonePrefab;

    public float tiltAroundX;
    public float tiltAroundZ;

    bool isSpreading;

    public List<GameObject> spottedPheromnes;
    public Vector3 averagePheromonesLocationPointInWorldPosition = new Vector3(0, 0, 0);
    public Vector3 averagePheromonesForwardDirectionInWorldPosition = new Vector3(0, 0, 0);

    public GameObject averagePheromonesLocationPrefab;
    public GameObject averagePheromonesDirectionPrefab;
    public GameObject myAveragePheromonesLocationPrefab;
    public GameObject myAveragePheromonesDirectionPrefab;

    public Vector3 myForwardDirecton;
    public bool flippedDirection;

    private void Start()
    {
        isSpreading = false;
        isScouting = true;
        startTime = Time.time;
        timeInDirection = 0;
        flippedDirection = false;

        myPheromoneSpredRate = PheromoneSpredRate.Normal;

        RandomizeDirection();
        isInGatheringCooldown = false;
        //rangeCalculationPoint = transform.position;
        gathererParent = gameObject.GetComponentInParent<Unit>();
    }

    public IEnumerator SpreadPheromone()
    {
        isSpreading = true;
        yield return new WaitForSeconds((int)myPheromoneSpredRate * Pheromone_Spread_Default_Rate);
        GameObject spreadedPheromone = Instantiate(pheromonePrefab, transform.position, Quaternion.LookRotation(transform.forward));
        spreadedPheromone.GetComponent<Collider>().enabled = false;
        spreadedPheromone.GetComponent<Collider>().enabled = true;
        //(carryingAmount>0 || targetResource != null)
        if (carryingAmount>0)
        {
            spreadedPheromone.GetComponent<Pheromone>().SetPheromoneType(Pheromone.PheromoneType.ToFood);
        }
        else
        {
            spreadedPheromone.GetComponent<Pheromone>().SetPheromoneType(Pheromone.PheromoneType.ToHome);
        }
        spottedPheromnes.Remove(spreadedPheromone);
        yield return new WaitForSeconds(.5f);
        isSpreading = false;
    }

    private void Update()
    {
        myForwardDirecton = transform.forward;
        if (!isSpreading)
        {
            StartCoroutine(SpreadPheromone());
        }
        
        if (!isScouting)
        {
            if (targetResource != null)
            {
                if (Vector3.Distance(this.transform.position, targetResource.transform.position) < 0.1f)
                {
                    Gather(targetResource);
                    if (isFull)
                    {
                        targetResource = null;
                        isScouting = true;
                        FlipDirection();
                    }
                }
                else
                {
                    WalkTowards();
                }
            }
            else if (targetResourceSilo != null)
            {
                if (Vector3.Distance(this.transform.position, targetResourceSilo.transform.position) < 0.1f)
                {
                    Gather(targetResource);
                    if (carryingAmount == 0) 
                    {
                        targetResourceSilo = null;
                        isScouting = true;
                        FlipDirection();
                    }
                }
                else
                {
                    WalkTowards();
                }
            }
        }
        else
        {
            myPheromoneSpredRate = PheromoneSpredRate.Normal;
            if (timeInDirection <= 0 || startTime + timeInDirection <= Time.time)
            {
                timeInDirection = Random.Range(minRandomTime, maxRandomTime);
                startTime = Time.time;

                if (carryingAmount > 0)
                {
                    averagePheromonesLocationPointInWorldPosition = GetAveragePheromonPointByType(Pheromone.PheromoneType.ToHome);
                    averagePheromonesForwardDirectionInWorldPosition = GetAveragePheromonDirectionByType(Pheromone.PheromoneType.ToHome);
                }
                else
                {
                    averagePheromonesLocationPointInWorldPosition = GetAveragePheromonPointByType(Pheromone.PheromoneType.ToFood);
                    averagePheromonesForwardDirectionInWorldPosition = GetAveragePheromonDirectionByType(Pheromone.PheromoneType.ToFood);
                }

                UpdatePheromonesIdentifiorHelper();
                Quaternion pheromoneLocation = Quaternion.LookRotation(averagePheromonesLocationPointInWorldPosition, Vector3.up);
                Quaternion pheromoneDirection = Quaternion.LookRotation(averagePheromonesForwardDirectionInWorldPosition, Vector3.up);
                pheromoneDirection = FlipQuaternion();

                if (!flippedDirection)
                {
                    tiltAroundX = KeepNumberInRange(tiltAroundX + Random.Range(Min_Interval_Direction, Max_Interval_Direction), Min_Value_Direction, Max_Value_Direction);
                    tiltAroundZ = KeepNumberInRange(tiltAroundZ + Random.Range(Min_Interval_Direction, Max_Interval_Direction), Min_Value_Direction, Max_Value_Direction);
                    flippedDirection = false;
                }


                quaternionTarget = Quaternion.LookRotation(new Vector3(tiltAroundX, 0, tiltAroundZ));
                if (pheromoneLocation != new Quaternion(0, 0, 0, 1))
                {
                    //quaternionTarget = Quaternion.Lerp(pheromoneLocation, quaternionTarget, 0.1f);
                    quaternionTarget = Quaternion.Lerp(pheromoneDirection, quaternionTarget, 0.1f);
                    //quaternionTarget = Quaternion.Lerp(pheromoneDirection, Quaternion.Inverse(quaternionTarget), 0.1f);
                }
            }
            transform.Translate(Vector3.forward * Time.deltaTime * unitDetails.speed);
            transform.rotation = Quaternion.Slerp(transform.rotation, quaternionTarget, Time.deltaTime * unitDetails.rotation_speed);
        }
    }

    private void RandomizeDirection()
    {
        tiltAroundX = Random.Range(Min_Value_Direction, Max_Value_Direction);
        tiltAroundZ = Random.Range(Min_Value_Direction, Max_Value_Direction);
    }

    private void FlipDirection()
    {
        float tmp = tiltAroundX;
        tiltAroundX = -tiltAroundZ;
        tiltAroundZ = tmp;
        flippedDirection = true;
    }

    private Quaternion FlipQuaternion()
    {
        Quaternion toReverse = Quaternion.LookRotation(averagePheromonesForwardDirectionInWorldPosition);

        Vector3 rot = toReverse.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);

        return Quaternion.Euler(rot);
    }

    private float KeepNumberInRange(float number, float min, float max)
    {
        if (number > max)
        {
            float offset = max - number;
            return max - offset;
        }
        else if (number < min)
        {
            float offset = min - number;
            return min - offset;
        }
        return number;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Pheromone>())
        {
            spottedPheromnes.Add(other.gameObject);
            if (carryingAmount > 0)
            {
                averagePheromonesLocationPointInWorldPosition = GetAveragePheromonPointByType(Pheromone.PheromoneType.ToHome);
                averagePheromonesForwardDirectionInWorldPosition = GetAveragePheromonDirectionByType(Pheromone.PheromoneType.ToHome);
            }
            else
            {
                averagePheromonesLocationPointInWorldPosition = GetAveragePheromonPointByType(Pheromone.PheromoneType.ToFood);
                averagePheromonesForwardDirectionInWorldPosition = GetAveragePheromonDirectionByType(Pheromone.PheromoneType.ToFood);
            }
            UpdatePheromonesIdentifiorHelper();
        }

        if (!isFull)
        {
            if (targetResource == null && other.GetComponent<Resource>())
            {
                myPheromoneSpredRate = PheromoneSpredRate.Rapid;
                targetResource = other.GetComponent<Resource>();
                targetPoint = other.transform.position;
                isScouting = false;
                DestroyAveragePheromones();
            }
        }
        else
        {
            if (carryingAmount>0 && other.GetComponent<ResourceSilo>())
            {
                myPheromoneSpredRate = PheromoneSpredRate.Rapid;
                targetResourceSilo = other.GetComponent<ResourceSilo>();
                targetPoint = other.transform.position;
                isScouting = false;
                DestroyAveragePheromones();
            }
        }
    }

    private Vector3 GetAveragePheromonPointByType(Pheromone.PheromoneType pheromoneType)
    {
        Vector3 ans = new Vector3(0,0,0);
        int count = 0;
        foreach (GameObject pheromone in spottedPheromnes)
        {
            if(pheromone.GetComponent<Pheromone>().myPheromoneType == pheromoneType)
            {
                ans += new Vector3(pheromone.transform.position.x, 0f, pheromone.transform.position.z);
                count++;
            }
        }
        if (count > 0)
        {
            ans = new Vector3(ans.x / count, ans.y / count, ans.z / count);
        }

        

        return ans;
    }

    private void DestroyAveragePheromones()
    {
        GameObject.Destroy(myAveragePheromonesDirectionPrefab);
        GameObject.Destroy(myAveragePheromonesLocationPrefab);
    }

    private void UpdatePheromonesIdentifiorHelper()
    {
        if (myPlayer.debugMode)
        {
            DestroyAveragePheromones();
            if (averagePheromonesLocationPointInWorldPosition!=new Vector3(0, 0, 0))
            {
                myAveragePheromonesLocationPrefab = Instantiate(averagePheromonesLocationPrefab, averagePheromonesLocationPointInWorldPosition, Quaternion.LookRotation(averagePheromonesForwardDirectionInWorldPosition));
                //myAveragePheromonesDirectionPrefab = Instantiate(averagePheromonesDirectionPrefab, averagePheromonesLocationPointInWorldPosition, Quaternion.LookRotation(averagePheromonesForwardDirectionInWorldPosition));
                Quaternion toReverse = Quaternion.LookRotation(averagePheromonesForwardDirectionInWorldPosition);

                Vector3 rot = toReverse.eulerAngles;
                rot = new Vector3(rot.x, rot.y + 180, rot.z);

                myAveragePheromonesDirectionPrefab = Instantiate(averagePheromonesDirectionPrefab, averagePheromonesLocationPointInWorldPosition, Quaternion.Euler(rot));
            }
        }
    }

    private Vector3 GetAveragePheromonDirectionByType(Pheromone.PheromoneType pheromoneType)
    {
        Vector3 ans = new Vector3(0, 0, 0);
        int count = 0;
        foreach (GameObject pheromone in spottedPheromnes)
        {
            if (pheromone.GetComponent<Pheromone>().myPheromoneType == pheromoneType)
            {
                ans += new Vector3(pheromone.transform.forward.x, 0f, pheromone.transform.forward.z);
                count++;
            }
        }
        ans = ans.normalized;

        return ans;
    }

    private void OnTriggerExit(Collider other)
    {
        if (targetResource != null)
        {
            if (other.GetComponent<Resource>() == targetResource)
            {
                //find other resources in view
                targetResource = null;
                hasTarget = false;
            }
        }

        if (other.GetComponent<Pheromone>())
        {
            spottedPheromnes.Remove(other.gameObject);
            if (carryingAmount > 0)
            {
                averagePheromonesLocationPointInWorldPosition = GetAveragePheromonPointByType(Pheromone.PheromoneType.ToHome);
                averagePheromonesForwardDirectionInWorldPosition = GetAveragePheromonDirectionByType(Pheromone.PheromoneType.ToHome);
            }
            else
            {
                averagePheromonesLocationPointInWorldPosition = GetAveragePheromonPointByType(Pheromone.PheromoneType.ToFood);
                averagePheromonesForwardDirectionInWorldPosition = GetAveragePheromonDirectionByType(Pheromone.PheromoneType.ToFood);
            }
            UpdatePheromonesIdentifiorHelper();
        }
    }

    void WalkTowards()
    {
        if (targetResource != null)
        {
            targetPoint = targetResource.transform.position;
        }
        else if (targetResourceSilo != null)
        {
            targetPoint = targetResourceSilo.transform.position;
        }
        Vector3 targetDirection = targetPoint - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, unitDetails.rotation_speed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, unitDetails.speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            hasTarget = false;
        }
    }
}

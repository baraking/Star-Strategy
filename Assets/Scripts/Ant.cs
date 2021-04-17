using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Notice the collider is a sphere and not a cone or something.
//find other resources in view in case the cur one was moved / destroyed
public class Ant : Gatherer
{
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
    public Vector3 averagePheromonesLocationPointInWorldPosition = new Vector3(0,0,0);

    private void Start()
    {
        isSpreading = false;
        isScouting = true;
        startTime = Time.time;
        timeInDirection = 0;

        RandomizeDirection();
        isInGatheringCooldown = false;
        //rangeCalculationPoint = transform.position;
        gathererParent = gameObject.GetComponentInParent<Unit>();
    }

    public IEnumerator SpreadPheromone()
    {
        isSpreading = true;
        yield return new WaitForSeconds(.5f);
        GameObject spreadedPheromone = Instantiate(pheromonePrefab, transform.position, new Quaternion());
        if (isFull || targetResource != null)
        {
            spreadedPheromone.GetComponent<Pheromone>().SetPheromoneType(Pheromone.PheromoneType.ToFood);
        }
        else
        {
            spreadedPheromone.GetComponent<Pheromone>().SetPheromoneType(Pheromone.PheromoneType.ToHome);
        }
        yield return new WaitForSeconds(.5f);
        isSpreading = false;
    }

    private void Update()
    {
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
            if (timeInDirection <= 0 || startTime + timeInDirection <= Time.time)
            {
                timeInDirection = Random.Range(minRandomTime, maxRandomTime);
                startTime = Time.time;

                averagePheromonesLocationPointInWorldPosition = GetAveragePheromonPoint();

                tiltAroundX = KeepNumberInRange(tiltAroundX + Random.Range(Min_Interval_Direction, Max_Interval_Direction),Min_Value_Direction, Max_Value_Direction);
                tiltAroundZ = KeepNumberInRange(tiltAroundZ + Random.Range(Min_Interval_Direction, Max_Interval_Direction), Min_Value_Direction, Max_Value_Direction);

                quaternionTarget = Quaternion.LookRotation(new Vector3(tiltAroundX, 0, tiltAroundZ));
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
        if (carryingAmount == 0 && targetResource == null)
        {
            if (other.GetComponent<Pheromone>())
            {
                if (other.GetComponent<Pheromone>().myPheromoneType == Pheromone.PheromoneType.ToFood)
                {
                    spottedPheromnes.Add(other.gameObject);
                    averagePheromonesLocationPointInWorldPosition = GetAveragePheromonPoint();
                }
            }
        }

        if (!isFull)
        {
            if (targetResource == null && other.GetComponent<Resource>())
            {
                targetResource = other.GetComponent<Resource>();
                targetPoint = other.transform.position;
                isScouting = false;
            }
        }
        else
        {
            if (carryingAmount>0 && other.GetComponent<ResourceSilo>())
            {
                print(this.gameObject.name + " sees a Silo: " + other);
                targetResourceSilo = other.GetComponent<ResourceSilo>();
                targetPoint = other.transform.position;
                isScouting = false;
            }
        }
    }

    private Vector3 GetAveragePheromonPoint()
    {
        Vector3 ans = new Vector3(0,0,0);
        foreach (GameObject pheromone in spottedPheromnes)
        {
            ans += new Vector3(pheromone.transform.position.x, 0f, pheromone.transform.position.z);
            
        }
        ans = new Vector3(ans.x / spottedPheromnes.Count, ans.y / spottedPheromnes.Count, ans.z / spottedPheromnes.Count);
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
            averagePheromonesLocationPointInWorldPosition = GetAveragePheromonPoint();
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

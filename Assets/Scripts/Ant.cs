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

    public float tiltAroundX;
    public float tiltAroundZ;

    private void Start()
    {
        isScouting = true;
        startTime = Time.time;
        timeInDirection = 0;

        RandomizeDirection();
        isInGatheringCooldown = false;
        //rangeCalculationPoint = transform.position;
        gathererParent = gameObject.GetComponentInParent<Unit>();
    }

    private void Update()
    {
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
        }
        else
        {
            if (timeInDirection <= 0 || startTime + timeInDirection <= Time.time)
            {
                timeInDirection = Random.Range(minRandomTime, maxRandomTime);
                startTime = Time.time;

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
                targetResourceSilo = other.GetComponent<ResourceSilo>();
                targetPoint = other.transform.position;
                isScouting = false;
            }
        }
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
    }

    void WalkTowards()
    {
        targetPoint = targetResource.transform.position;
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

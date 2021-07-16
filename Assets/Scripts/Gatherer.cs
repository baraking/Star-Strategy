using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GetClosestResourceSilo distance should be related to path and not actual aireal distance.
//auto gathering - especially once a resource is depleted
public class Gatherer : Walkable
{
    public bool isInGatheringCooldown;
    public bool isFull;
    public int carryingAmount;

    public Resource targetResource;
    public ResourceSilo targetResourceSilo;

    void Start()
    {
        base.InitUnit();
        isInGatheringCooldown = false;
        //rangeCalculationPoint = transform.position;
    }

    public void Gather(Resource target)
    {
        if (targetResourceSilo == null)
        {
            //if (!isInCooldown && Vector3.Distance(rangeCalculationPoint, targetUnit.transform.position) <= weaponDetails.range)
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget <= unitDetails.gatheringRange)
            {
                if (!isInGatheringCooldown)
                {
                    if(unitDetails.gatheringCapacity - carryingAmount - unitDetails.gatherAmount < 0)
                    {
                        carryingAmount += target.GiveResources(unitDetails.gatheringCapacity - carryingAmount);
                        isFull = true;
                        GetClosestResourceSilo();
                    }
                    else
                    {
                        carryingAmount += target.GiveResources(unitDetails.gatherAmount);
                    }
                    isInGatheringCooldown = true;
                    StartCoroutine(AfterGather());
                }
            }
            else
            {
                if (GetTargetPoint() == Vector3.zero)
                {
                    print("start moving!");
                    SetTargetPoint(transform.position - (target.transform.position.normalized * distanceToTarget));
                }
            }
        }
        else if(targetResourceSilo != null)
        {
            SetHasTarget(true);
            float distanceToTarget = Vector3.Distance(transform.position, targetResourceSilo.transform.position);
            SetTargetPoint(targetResourceSilo.transform.position);
            if (distanceToTarget <= unitDetails.gatheringRange)
            {
                isFull = false;
                myPlayer.resources += carryingAmount;
                carryingAmount = 0;
                targetResourceSilo = null;
                if (targetResource != null)
                {
                    SetHasTarget(true);
                    SetTargetPoint(targetResource.transform.position);
                }
            }
            else
            {
                if (GetTargetPoint() == Vector3.zero)
                {
                    print("start moving!");
                    SetTargetPoint(transform.position - (targetResourceSilo.transform.position.normalized * distanceToTarget));
                }
            }
        }
    }

    public void GetClosestResourceSilo()
    {
        foreach (Unit resourceSilo in myPlayer.playerUnits)
        {
            if (resourceSilo.GetComponent<ResourceSilo>())
            {
                if (targetResourceSilo == null)
                {
                    targetResourceSilo = resourceSilo.GetComponent<ResourceSilo>();
                }
                else if(Vector3.Distance(this.transform.position, targetResourceSilo.transform.position)> Vector3.Distance(this.transform.position, resourceSilo.transform.position))
                {
                    targetResourceSilo = resourceSilo.GetComponent<ResourceSilo>();
                }
            }
        }
        //print(targetResourceSilo.name);
    }

    public IEnumerator AfterGather()
    {
        yield return new WaitForSeconds(unitDetails.gatheringCooldown);
        isInGatheringCooldown = false;
    }
}

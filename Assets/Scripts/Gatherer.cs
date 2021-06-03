using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GetClosestResourceSilo distance should be related to path and not actual aireal distance.
//auto gathering - especially once a resource is depleted
//check the gathererParent
public class Gatherer : Walkable
{
    public bool isInGatheringCooldown;
    public bool isFull;
    public int carryingAmount;
    public Unit gathererParent;

    public Resource targetResource;
    public ResourceSilo targetResourceSilo;

    void Start()
    {
        isInGatheringCooldown = false;
        //rangeCalculationPoint = transform.position;
        gathererParent = gameObject.GetComponentInParent<Unit>();
    }

    private void Update()
    {
        base.AttempotToWalk();
        if (targetResource != null)
        {
            Gather(targetResource);
        }
    }

    public void Gather(Resource target)
    {
        if (targetResourceSilo == null)
        {
            //if (!isInCooldown && Vector3.Distance(rangeCalculationPoint, targetUnit.transform.position) <= weaponDetails.range)
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget <= gathererParent.unitDetails.gatheringRange)
            {
                if (!isInGatheringCooldown)
                {
                    if(gathererParent.unitDetails.gatheringCapacity - carryingAmount - gathererParent.unitDetails.gatherAmount < 0)
                    {
                        carryingAmount += target.GiveResources(gathererParent.unitDetails.gatheringCapacity - carryingAmount);
                        isFull = true;
                        GetClosestResourceSilo();
                    }
                    else
                    {
                        carryingAmount += target.GiveResources(gathererParent.unitDetails.gatherAmount);
                    }
                    isInGatheringCooldown = true;
                    StartCoroutine(AfterGather());
                }
            }
            else
            {
                if (gathererParent.GetComponent<Walkable>())
                {
                    if (gathererParent.GetComponent<Walkable>().GetTargetPoint() == Vector3.zero)
                    {
                        print("start moving!");
                        gathererParent.GetComponent<Walkable>().SetTargetPoint(transform.position - (target.transform.position.normalized * distanceToTarget));
                    }
                }
            }
        }
        else if(targetResourceSilo != null)
        {
            gathererParent.GetComponent<Walkable>().SetHasTarget(true);
            float distanceToTarget = Vector3.Distance(transform.position, targetResourceSilo.transform.position);
            gathererParent.GetComponent<Walkable>().SetTargetPoint(targetResourceSilo.transform.position);
            if (distanceToTarget <= gathererParent.unitDetails.gatheringRange)
            {
                isFull = false;
                myPlayer.resources += carryingAmount;
                carryingAmount = 0;
                targetResourceSilo = null;
                if (targetResource != null)
                {
                    gathererParent.GetComponent<Walkable>().SetHasTarget(true);
                    gathererParent.GetComponent<Walkable>().SetTargetPoint(targetResource.transform.position);
                }
            }
            else
            {
                if (gathererParent.GetComponent<Walkable>())
                {
                    if (gathererParent.GetComponent<Walkable>().GetTargetPoint() == Vector3.zero)
                    {
                        print("start moving!");
                        gathererParent.GetComponent<Walkable>().SetTargetPoint(transform.position - (targetResourceSilo.transform.position.normalized * distanceToTarget));
                    }
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
        print(targetResourceSilo.name);
    }

    public IEnumerator AfterGather()
    {
        yield return new WaitForSeconds(gathererParent.unitDetails.gatheringCooldown);
        isInGatheringCooldown = false;
    }
}

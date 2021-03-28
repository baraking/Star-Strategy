using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : Walkable
{
    public bool isInGatheringCooldown;
    public bool isFull;
    public int carryingAmount;
    public Unit gathererParent;

    public Resource targetResource;

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
        if (!isFull)
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
                    if (gathererParent.GetComponent<Walkable>().targetPoint == Vector3.zero)
                    {
                        print("start moving!");
                        gathererParent.GetComponent<Walkable>().targetPoint = transform.position - (target.transform.position.normalized * distanceToTarget);
                    }
                }
            }
        }

    }

    public IEnumerator AfterGather()
    {
        yield return new WaitForSeconds(gathererParent.unitDetails.gatheringCooldown);
        isInGatheringCooldown = false;
    }
}

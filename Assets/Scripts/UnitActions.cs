using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//maybe should have a class like this for each unit type? - walkable,gatherer,ant etc.
//should fix the code so that Rotate wont need targetsLocation[0]
public class UnitActions : MonoBehaviour
{
    public static float ROTATION_THRESHOLD = 15f;

    public static void Rotate(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion, GameObject target)
    {
        Vector3 targetDir = targetsLocation[0] - actingUnit.transform.position;
        targetDir = targetDir.normalized;

        float dot = Vector3.Dot(targetDir, actingUnit.transform.forward);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (angle >= -1 && angle <= 1)
        {
            actingUnit.transform.rotation = Quaternion.Slerp(actingUnit.transform.rotation, endQuaternion, Time.deltaTime * actingUnit.unitDetails.rotation_speed);
        }
    }

    public static void Move(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion, GameObject target)
    {
        if (targetsLocation.Count < 1)
        {
            return;
        }

        if (Vector3.Distance(actingUnit.transform.position, targetsLocation[0]) < 0.05f)
        {
            if (targetsLocation.Count < 1)
            {
                actingUnit.unitAction = Rotate;
                return;
            }
            else
            {
                targetsLocation.RemoveAt(0);
            }
        }

        MoveLogic(actingUnit, targetsLocation, endQuaternion);
    }

    private static void MoveLogic(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion)
    {
        Vector3 targetDir = targetsLocation[0] - actingUnit.transform.position;
        targetDir = targetDir.normalized;

        float dot = Vector3.Dot(targetDir, actingUnit.transform.forward);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        Quaternion targetDirection = Quaternion.LookRotation(new Vector3(targetDir.x, 0, targetDir.z).normalized, Vector3.up);

        if (angle > ROTATION_THRESHOLD * 2 || angle < -ROTATION_THRESHOLD * 2)
        {
            actingUnit.transform.rotation = Quaternion.Slerp(actingUnit.transform.rotation, targetDirection, Time.deltaTime * actingUnit.unitDetails.rotation_speed);
        }
        else if ((angle > ROTATION_THRESHOLD && angle <= ROTATION_THRESHOLD * 2) || (angle < -ROTATION_THRESHOLD && angle >= -ROTATION_THRESHOLD * 2))
        {
            actingUnit.transform.rotation = Quaternion.Slerp(actingUnit.transform.rotation, targetDirection, Time.deltaTime * actingUnit.unitDetails.rotation_speed);
            actingUnit.transform.Translate(Vector3.forward * Time.deltaTime * actingUnit.unitDetails.speed * .5f);
        }
        else if ((angle > 1 && angle <= ROTATION_THRESHOLD) || (angle < -1 && angle >= ROTATION_THRESHOLD))
        {
            actingUnit.transform.rotation = Quaternion.Slerp(actingUnit.transform.rotation, targetDirection, Time.deltaTime * actingUnit.unitDetails.rotation_speed);
            actingUnit.transform.Translate(Vector3.forward * Time.deltaTime * actingUnit.unitDetails.speed);
        }
        else if (angle >= -1 && angle <= 1)
        {
            actingUnit.transform.Translate(Vector3.forward * Time.deltaTime * actingUnit.unitDetails.speed);
        }
    }

    private static Quaternion FlipQuaternion(Vector3 pos)
    {
        Quaternion toReverse = Quaternion.LookRotation(pos);

        Vector3 rot = toReverse.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);

        return Quaternion.Euler(rot);
    }

    public static void Patrol(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion, GameObject target)
    {
        if (targetsLocation.Count < 1)
        {
            return;
        }

        if (Vector3.Distance(actingUnit.transform.position, targetsLocation[0]) < 0.05f)
        {
            if (targetsLocation.Count < 1)
            {
                actingUnit.unitAction = Rotate;
                return;
            }
            else
            {
                Vector3 putAtLast = targetsLocation[0];
                targetsLocation.RemoveAt(0);
                targetsLocation.Add(putAtLast);
            }
        }

        MoveLogic(actingUnit, targetsLocation, endQuaternion);
    }

    public static void Advance(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion, GameObject target)//move and attack when in range
    {
        //print(actingUnit.GetShortestRangeOfWeapons() + " , " + Vector3.Distance(actingUnit.transform.position, target.transform.position));
        if (actingUnit.GetShortestRangeOfWeapons() < Vector3.Distance(actingUnit.transform.position, target.transform.position))
        {
            Move(actingUnit, targetsLocation, endQuaternion,target);
        }
        /*else
        {
            actingUnit.unitAction = Attack;
        }*/
        Attack(actingUnit, targetsLocation, endQuaternion, target);
    }

    public static void Attack(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion, GameObject target)
    {
        if (target == null || actingUnit.unitWeapons.Count<1)
        {
            return;
        }

        foreach(Weapon weapon in actingUnit.unitWeapons)
        {
            if(Vector3.Distance(weapon.transform.position, target.transform.position) <= weapon.weaponDetails.range)
            {
                actingUnit.Fire(target.GetComponent<Unit>(), weapon);
            }
        }
    }

    public static void Gather(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion, GameObject target)
    {
        if (!target.GetComponent<Resource>())
        {
            return;
        }
        //print(Vector3.Distance(actingUnit.transform.position, target.transform.position) + " , " + actingUnit.unitDetails.gatheringRange);
        if (Vector3.Distance(actingUnit.transform.position, target.transform.position) > actingUnit.unitDetails.gatheringRange)
        {
            Move(actingUnit, targetsLocation, endQuaternion, target);
        }
        else
        {
            if (!actingUnit.GetComponent<Gatherer>().isInGatheringCooldown)
            {
                if (actingUnit.unitDetails.gatheringCapacity - actingUnit.GetComponent<Gatherer>().carryingAmount - actingUnit.unitDetails.gatherAmount < 0)
                {
                    actingUnit.GetComponent<Gatherer>().carryingAmount += target.GetComponent<Resource>().GiveResources(actingUnit.unitDetails.gatheringCapacity - actingUnit.GetComponent<Gatherer>().carryingAmount);
                    actingUnit.GetComponent<Gatherer>().isFull = true;
                    actingUnit.GetComponent<Gatherer>().GetClosestResourceSilo();

                    actingUnit.actionTarget = actingUnit.GetComponent<Gatherer>().targetResourceSilo.gameObject;
                    actingUnit.targetsLocation = new List<Vector3>() { actingUnit.GetComponent<Gatherer>().targetResourceSilo.transform.position };
                    actingUnit.unitAction = RetrieveResources;
                }
                else
                {
                    actingUnit.GetComponent<Gatherer>().carryingAmount += target.GetComponent<Resource>().GiveResources(actingUnit.unitDetails.gatherAmount);
                    actingUnit.GetComponent<Gatherer>().isInGatheringCooldown = true;
                    actingUnit.Wait(Gather, actingUnit.unitDetails.gatheringCooldown);
                }
            }
            else
            {
                if (!actingUnit.isWaiting)
                {
                    actingUnit.GetComponent<Gatherer>().isInGatheringCooldown = false;
                }
            }
        }
    }

    public static void RetrieveResources(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion, GameObject target)
    {
        if (!target.GetComponent<ResourceSilo>())
        {
            return;
        }
        if (Vector3.Distance(actingUnit.transform.position, target.transform.position) > actingUnit.unitDetails.gatheringRange)
        {
            Move(actingUnit, targetsLocation, endQuaternion, target);
        }
        else
        {
            actingUnit.Wait(Gather, actingUnit.unitDetails.gatheringCooldown);
            actingUnit.GetComponent<Gatherer>().isFull = false;
            actingUnit.myPlayer.resources += actingUnit.GetComponent<Gatherer>().carryingAmount;
            actingUnit.GetComponent<Gatherer>().carryingAmount = 0;
            actingUnit.GetComponent<Gatherer>().targetResourceSilo = null;
            if (actingUnit.GetComponent<Gatherer>().targetResource != null)
            {
                actingUnit.actionTarget = actingUnit.GetComponent<Gatherer>().targetResource.gameObject;
                actingUnit.targetsLocation = new List<Vector3>() { actingUnit.GetComponent<Gatherer>().targetResource.transform.position };
                actingUnit.unitAction = Gather;
            }
        }
    }

    public static void Ram(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion, GameObject target)
    {

    }

    public static void Idle(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion, GameObject target)
    {

    }

    //build

    //embark

    //disembark
}

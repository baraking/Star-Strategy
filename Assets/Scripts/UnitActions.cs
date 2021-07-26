using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//maybe should have a class like this for each unit type? - walkable,gatherer,ant etc.
//should fix the code so that Rotate wont need targetsLocation[0]
public class UnitActions : MonoBehaviour
{
    public static float ROTATION_THRESHOLD = 15f;

    public static void Rotate(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)
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

    public static void Move(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)
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

        MoveLogic(actingUnit, endQuaternion, targetsLocation);
    }

    private static void MoveLogic(Unit actingUnit, Quaternion endQuaternion, List<Vector3> targetsLocation)
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

    public static void Patrol(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)
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

        MoveLogic(actingUnit, endQuaternion, targetsLocation);
    }

    public static void Advance(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)//move and attack when in range
    {
        //print(actingUnit.GetShortestRangeOfWeapons() + " , " + Vector3.Distance(actingUnit.transform.position, target.transform.position));
        if (actingUnit.GetShortestRangeOfWeapons() < Vector3.Distance(actingUnit.transform.position, target.transform.position))
        {
            Move(actingUnit, target, endQuaternion, targetsLocation);

            foreach (Weapon weapon in actingUnit.unitWeapons)
            {
                if (Vector3.Distance(weapon.transform.position, target.transform.position) > weapon.weaponDetails.range)
                {
                    weapon.targetUnit = target.GetComponent<Unit>();
                    weapon.weaponAction = WeaponActions.RotateWeapon;
                }
            }
        }
        Move(actingUnit, target, endQuaternion, targetsLocation);
    }

    public static void Attack(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)
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

    public static void Gather(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)
    {
        if (!target.GetComponent<Resource>())
        {
            return;
        }
        //print(Vector3.Distance(actingUnit.transform.position, target.transform.position) + " , " + actingUnit.unitDetails.gatheringRange);
        if (Vector3.Distance(actingUnit.transform.position, target.transform.position) > actingUnit.unitDetails.gatheringRange)
        {
            Move(actingUnit, target, endQuaternion, targetsLocation);
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

    public static void RetrieveResources(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)
    {
        if (!target.GetComponent<ResourceSilo>())
        {
            return;
        }
        if (Vector3.Distance(actingUnit.transform.position, target.transform.position) > actingUnit.unitDetails.gatheringRange)
        {
            Move(actingUnit, target, endQuaternion, targetsLocation);
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

    public static void Ram(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)
    {

    }

    public static void Idle(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)
    {

    }

    public static void Spawn(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)
    {
        if (actingUnit.unitDetails.unitType == UnitDetails.UnitType.Building)
        {
            actingUnit.StartSpawningUnit();
        }
        else if (Vector3.Distance(actingUnit.transform.position, targetsLocation[0]) > actingUnit.unitDetails.gatheringRange)
        {
            Move(actingUnit, target, endQuaternion, targetsLocation);
        }
        else
        {
            actingUnit.StartSpawningUnit();
        }
    }

    public static void StartBuilding(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)
    {
        if (Vector3.Distance(actingUnit.transform.position, targetsLocation[0]) > actingUnit.unitDetails.gatheringRange)
        {
            Move(actingUnit, target, endQuaternion, targetsLocation);
        }
        else
        {
            actingUnit.StartSpawningBuilding();
            actingUnit.unitAction = Build;
        }
    }

    public static void Build(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)
    {
        if (Vector3.Distance(actingUnit.transform.position, targetsLocation[0]) > actingUnit.unitDetails.gatheringRange)
        {
            Move(actingUnit, target, endQuaternion, targetsLocation);
        }
        else
        {
            actingUnit.Build();
        }
    }

    //embark

    //disembark
}

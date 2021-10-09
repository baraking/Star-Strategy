using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//maybe should have a class like this for each unit type? - walkable,gatherer,ant etc.
//should fix the code so that Rotate wont need targetsLocation[0]
//attacking a building while it is being built may cause an endless build
public class UnitActions : MonoBehaviour
{
    public enum PossibleAction { Rotate, Move, Patrol, Advance, Attack, Gather, RetrieveResources, Ram, Idle , Spawn , StartBuilding , Build , Produce }

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
        //print(actingUnit + " is heading to attack!");
        //print(actingUnit.GetShortestRangeOfWeapons() + " , " + Vector3.Distance(actingUnit.transform.position, target.transform.position));
        if (actingUnit.GetShortestRangeOfWeapons() < Vector3.Distance(actingUnit.transform.position, target.transform.position))
        {
            Move(actingUnit, target, endQuaternion, targetsLocation);

            foreach (Weapon weapon in actingUnit.unitWeapons)
            {
                //print(weapon + ", range:" + weapon.weaponDetails.range + ", distance: " + Vector3.Distance(weapon.transform.position, target.transform.position));
                if (Vector3.Distance(weapon.transform.position, target.transform.position) > weapon.weaponDetails.range)
                {
                    weapon.targetUnit = target.GetComponent<Unit>();
                    weapon.weaponAction = WeaponActions.RotateWeapon;
                }
            }
        }
        Attack(actingUnit, target, endQuaternion, targetsLocation);

        targetsLocation[targetsLocation.Count - 1] = target.transform.position;
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

                    if (target.GetComponent<Resource>().curValue <= 0)
                    {
                        target.GetComponent<Resource>().OnDepleted();
                        actingUnit.GetComponent<Gatherer>().targetResource = actingUnit.GetComponent<Gatherer>().targetResourceParent.GetRandomResourceFromSpawner(target.GetComponent<Resource>().indexInParent, actingUnit.photonID);
                    }
                }
                else
                {
                    actingUnit.GetComponent<Gatherer>().carryingAmount += target.GetComponent<Resource>().GiveResources(actingUnit.unitDetails.gatherAmount);
                    actingUnit.GetComponent<Gatherer>().isInGatheringCooldown = true;
                    actingUnit.Wait(Gather, actingUnit.unitDetails.gatheringCooldown);

                    if (target.GetComponent<Resource>().curValue <= 0)
                    {
                        target.GetComponent<Resource>().OnDepleted();
                        actingUnit.GetComponent<Gatherer>().targetResource = actingUnit.GetComponent<Gatherer>().targetResourceParent.GetRandomResourceFromSpawner(target.GetComponent<Resource>().indexInParent, actingUnit.photonID);
                        actingUnit.actionTarget = actingUnit.GetComponent<Gatherer>().targetResource.gameObject;
                        actingUnit.targetsLocation = new List<Vector3>() { actingUnit.GetComponent<Gatherer>().targetResource.transform.position };
                    }
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
            actingUnit.myPlayer.AddResources(actingUnit.GetComponent<Gatherer>().carryingAmount);
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
            actingUnit.unitAction = Produce;
            actingUnit.myPlayer.UpdateUnitAction(actingUnit);
        }
        else if (Vector3.Distance(actingUnit.transform.position, targetsLocation[0]) > actingUnit.unitDetails.gatheringRange)
        {
            Move(actingUnit, target, endQuaternion, targetsLocation);
        }
        else
        {
            actingUnit.StartSpawningUnit();
            actingUnit.unitAction = Produce;
            actingUnit.myPlayer.UpdateUnitAction(actingUnit);
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
            if (actingUnit.GetComponent<Unit>().myPlayer.resources >= target.GetComponent<Purchasables>().GetPrice())
            {
                actingUnit.GetComponent<Unit>().myPlayer.AddResources(-target.GetComponent<Purchasables>().GetPrice());
            }
            else
            {
                actingUnit.unitAction = Idle;
                return;
            }
            actingUnit.StartSpawningBuilding();
            actingUnit.unitAction = Build;
            actingUnit.myPlayer.UpdateUnitAction(actingUnit);
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

    public static void Produce(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation)
    {
         actingUnit.ProduceUnit();
    }

    //embark

    //disembark

    public static int GetNumberFromUnitAction(Unit.UnitAction action)
    {
        if (action == UnitActions.Rotate)
        {
            return 1;
        }
        else if (action == UnitActions.Move)
        {
            return 2;
        }
        else if (action == UnitActions.Patrol)
        {
            return 3;
        }
        else if (action == UnitActions.Advance)
        {
            return 4;
        }
        else if (action == UnitActions.Attack)
        {
            return 5;
        }
        else if (action == UnitActions.Gather)
        {
            return 6;
        }
        else if (action == UnitActions.RetrieveResources)
        {
            return 7;
        }
        else if (action == UnitActions.Ram)
        {
            return 8;
        }
        else if (action == UnitActions.Idle)
        {
            return 9;
        }
        else if (action == UnitActions.Spawn)
        {
            return 10;
        }
        else if (action == UnitActions.StartBuilding)
        {
            return 11;
        }
        else if (action == UnitActions.Build)
        {
            return 12;
        }
        else if (action == UnitActions.Produce)
        {
            return 13;
        }
        else
        {
            return -1;
        }
    }

    public static Unit.UnitAction GetUnitActionFromNumber(int number)
    {
        if (number == 1)
        {
            return UnitActions.Rotate;
        }
        else if (number == 2)
        {
            return UnitActions.Move;
        }
        else if (number == 3)
        {
            return UnitActions.Patrol;
        }
        else if (number == 4)
        {
            return UnitActions.Advance;
        }
        else if (number == 5)
        {
            return UnitActions.Attack;
        }
        else if (number == 6)
        {
            return UnitActions.Gather;
        }
        else if (number == 7)
        {
            return UnitActions.RetrieveResources;
        }
        else if (number == 8)
        {
            return UnitActions.Ram;
        }
        else if (number == 9)
        {
            return UnitActions.Idle;
        }
        else if (number == 10)
        {
            return UnitActions.Spawn;
        }
        else if (number == 11)
        {
            return UnitActions.StartBuilding;
        }
        else if (number == 12)
        {
            return UnitActions.Build;
        }
        else if (number == 13)
        {
            return UnitActions.Produce;
        }
        else
        {
            return null;
        }
    }
}

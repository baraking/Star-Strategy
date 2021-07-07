using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//maybe should have a class like this for each unit type? - walkable,gatherer,ant etc.
//should fix the code so that Rotate wont need targetsLocation[0]
public class UnitActions : MonoBehaviour
{
    public static float ROTATION_THRESHOLD = 15f;

    public static void Rotate(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion)
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

    public static void Move(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion)
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

    public static void Patrol(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion)
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

    public static void Advance(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion)//move and attack when in range
    {
        if (actingUnit.GetShortestRangeOfWeapons() < Vector3.Distance(actingUnit.transform.position, targetsLocation[0]))
        {
            Move(actingUnit, targetsLocation, endQuaternion);
        }
        else
        {
            actingUnit.unitAction = Attack;
        }
    }

    public static void Attack(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion)
    {

    }

    public static void Harvest(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion)
    {

    }

    public static void RetrieveResources(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion)
    {

    }

    public static void Ram(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion)
    {

    }

    public static void Idle(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion)
    {

    }

    //build

    //embark

    //disembark
}

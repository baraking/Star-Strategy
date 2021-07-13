using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponActions : MonoBehaviour
{
    public static void RotateWeapon(Weapon actingWeapon,GameObject target)
    {
        Vector3 targetDirection = actingWeapon.targetUnit.transform.position - actingWeapon.transform.position;
        Vector3 newDirection = Vector3.RotateTowards(actingWeapon.transform.forward, targetDirection, 1f * Time.deltaTime, 0.0f);
        if (!actingWeapon.weaponDetails.shouldTurnToFire)
        {
            actingWeapon.transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else
        {
            actingWeapon.weaponParent.transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    public static void Fire(Weapon actingWeapon, GameObject target)
    {
        //delete this!
        RotateWeapon(actingWeapon, target);
        /*if()//rotation is not good enough
        {
            RotateWeapon(actingWeapon, target);
            return;
        }*/
        Vector3 targetDirection = actingWeapon.targetUnit.transform.position - actingWeapon.transform.position;
        print(actingWeapon.transform.forward + " , " + targetDirection +" : "+ (actingWeapon.transform.forward-targetDirection));

        float distanceToTarget = Vector3.Distance(actingWeapon.transform.position, target.transform.position);
        if (distanceToTarget <= actingWeapon.weaponDetails.range)
        {
            if (!actingWeapon.isInCooldown)
            {
                target.GetComponent<Unit>().TakeDamage(actingWeapon.weaponDetails.damage);
                actingWeapon.isInCooldown = true;

                actingWeapon.Wait(Fire, actingWeapon.weaponDetails.timeToShoot);
                //StartCoroutine(AfterFire());
            }
        }
    }

    public static void Scan(Weapon actingWeapon, GameObject target)
    {
        if (actingWeapon.enemiesAtRange.Count > 0)
        {
            Fire(actingWeapon, actingWeapon.enemiesAtRange[0].gameObject);
        }
    }

    public static void SteadyWeapon(Weapon actingWeapon, GameObject target)
    {
        Vector3 targetDirection = actingWeapon.gameObject.GetComponentInParent<Unit>().transform.forward;
        Vector3 newDirection = Vector3.RotateTowards(actingWeapon.transform.forward, targetDirection, 1f * Time.deltaTime, 0.0f);
        if (!actingWeapon.weaponDetails.shouldTurnToFire)
        {
            actingWeapon.transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else
        {
            actingWeapon.weaponParent.transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    public static void Reload(Weapon actingWeapon, GameObject target)
    {
        //StartCoroutine(Reload());
    }

    public static void Idle(Weapon actingWeapon, GameObject target)
    {

    }
}

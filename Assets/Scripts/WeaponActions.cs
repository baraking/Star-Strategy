using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponActions : MonoBehaviour
{
    public static void Fire(Weapon actingWeapon, GameObject target)
    {

    }

    public static void Scan(Weapon actingWeapon, GameObject target)
    {
        if (actingWeapon.enemiesAtRange.Count > 0)
        {
            Fire(actingWeapon, actingWeapon.enemiesAtRange[0].gameObject);
        }
    }

    public static void Reload(Weapon actingWeapon, GameObject target)
    {

    }

    public static void Idle(Weapon actingWeapon, GameObject target)
    {

    }
}

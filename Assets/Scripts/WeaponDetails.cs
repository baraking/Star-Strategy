using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public class WeaponDetails : PurchasableDetails
{
    public new string name;
    public float range;
    public int damage;
    public float timeToShoot;
    public float timeToReload;
    public int magazineSize;
    public bool shouldTurnToFire;

    public WeaponDetails(WeaponDetails other) : base(other)
    {
        CopyData(other);
    }

    public void CopyData(WeaponDetails other)
    {
        base.CopyData(other);

        this.name = other.name;
        this.range = other.range;
        this.damage = other.damage;
        this.timeToShoot = other.timeToShoot;
        this.timeToReload = other.timeToReload;
        this.magazineSize = other.magazineSize;
        this.shouldTurnToFire = other.shouldTurnToFire;
    }
}

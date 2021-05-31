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

    public float buildTime;
    public int costToBuild;

    public List<Purchasables> purchasables;

    public Sprite icon;
}

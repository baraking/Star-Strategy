using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//model
//...

[CreateAssetMenu(fileName ="New Unit", menuName ="Units")]
public class UnitDetails : PurchasableDetails
{
    public enum UnitType
    {
        Infantry, Vehicle, Flying, Monstruos, Building
    }

    public new string name;
    public UnitType unitType;
    public int unitSize;
    public int carryingCapacity;
    public int max_hp;

    public float speed;
    public float rotation_speed;

    public float buildTime;
    public int costToBuild;

    public int gatherAmount;
    public int gatheringCapacity;
    public float gatheringRange;
    public float gatheringCooldown;

    public List<Purchasables> purchasables;

    public List<int> prerequisites;
    public List<int> requirements;

    public Sprite icon;
}

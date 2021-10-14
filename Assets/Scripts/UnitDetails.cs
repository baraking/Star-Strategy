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

    public int gatherAmount;
    public int gatheringCapacity;
    public float gatheringRange;
    public float gatheringCooldown;

    public UnitDetails(UnitDetails other) : base(other)
    {
        this.name = other.name;
        this.unitType = other.unitType;
        this.unitSize = other.unitSize;
        this.carryingCapacity = other.carryingCapacity;
        this.max_hp = other.max_hp;

        this.speed = other.speed;
        this.rotation_speed = other.rotation_speed;

        this.gatherAmount = other.gatherAmount;
        this.gatheringCapacity = other.gatheringCapacity;
        this.gatheringRange = other.gatheringRange;
        this.gatheringCooldown = other.gatheringCooldown;
    }
}

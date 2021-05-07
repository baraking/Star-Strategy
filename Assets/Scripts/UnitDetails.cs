using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//model
//...

[CreateAssetMenu(fileName ="New Unit", menuName ="Units")]
public class UnitDetails : ScriptableObject
{
    public new string name;
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

    public Sprite icon;
}

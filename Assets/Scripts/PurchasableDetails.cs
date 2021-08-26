using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableDetails : ScriptableObject
{
    public float buildTime;
    public int costToBuild;
    public int price;

    public List<Purchasables> purchasables;

    public Sprite icon;

    public int[] prerequisites = new int[0];
    public int[] requirements = new int[0];
}

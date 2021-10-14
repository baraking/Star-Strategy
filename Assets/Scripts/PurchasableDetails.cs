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

    public PurchasableDetails(PurchasableDetails other)
    {
        this.buildTime = other.buildTime;
        this.costToBuild = other.costToBuild;
        this.price = other.price;

        foreach(Purchasables tmpPurchasables in other.purchasables)
        {
            this.purchasables.Add(tmpPurchasables);
        }
        this.icon = other.icon;

        this.prerequisites = new int[other.prerequisites.Length];
        for (int i = 0; i < this.prerequisites.Length; i++)
        {
            this.prerequisites[i] = other.prerequisites[i];
        }

        this.requirements = new int[other.requirements.Length];
        for (int i = 0; i < this.requirements.Length; i++)
        {
            this.requirements[i] = other.requirements[i];
        }
    }
}

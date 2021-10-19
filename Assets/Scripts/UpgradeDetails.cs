using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrades")]
public class UpgradeDetails : PurchasableDetails
{

    //need to create for each player a list with all of his units details, and use that.                        V

    //hold other purchasableDetails parameters with the changed value, and change for the specifec player only. V

    //have them as regular purchasables in buildings to buy                                                     V

    //after buy make all relevant purchaseables update thier purchaseablesDetails                               V

    //have a boolean array of all upgrades to not buy them a 2nd time

    public enum PossibleParameters { Max_hp, Damage }

    public int[] unitIndex;
    public PossibleParameters[] parameter;
    public float[] newValueMultiplier;

    public UpgradeDetails(PurchasableDetails other) : base(other)
    {

    }
}

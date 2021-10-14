using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Movement", menuName = "Movements")]
public class GroupMovementData : PurchasableDetails
{
    public Sprite icon;

    public GroupMovementData(PurchasableDetails other) : base(other)
    {

    }
}

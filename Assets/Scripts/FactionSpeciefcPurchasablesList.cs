using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FactionList", menuName = "FactionList")]
public class FactionSpeciefcPurchasablesList : ScriptableObject
{
    public PurchasableDetails[] details;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumansPlayerData : MonoBehaviour
{
    public bool[] landmarks;
    //index 0 - has Main Building
    //index 1 - has Tanks Factory
    public int arraySize = 2;

    public FactionSpeciefcPurchasablesList factionSpeciefcPurchasablesList;

    private void Awake()
    {
        landmarks = new bool[arraySize];

        for(int i=0;i< factionSpeciefcPurchasablesList.details.Length; i++)
        {
            if(factionSpeciefcPurchasablesList.details[i] is UnitDetails)
            {
                print("Unit!");
                factionSpeciefcPurchasablesList.details[i] = new UnitDetails((UnitDetails)factionSpeciefcPurchasablesList.details[i]);
            }
            else if (factionSpeciefcPurchasablesList.details[i] is WeaponDetails)
            {
                print("Weapon!");
                factionSpeciefcPurchasablesList.details[i] = new WeaponDetails(factionSpeciefcPurchasablesList.details[i]);
            }
            else
            {
                print("Purchasable!");
                factionSpeciefcPurchasablesList.details[i] = new PurchasableDetails(factionSpeciefcPurchasablesList.details[i]);
            }
            //foreach(var in ...)
            //factionSpeciefcPurchasablesList.details[i] = ScriptableObject.Instantiate(factionSpeciefcPurchasablesList.details[i]);
        }
    }
}

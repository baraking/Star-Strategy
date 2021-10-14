using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumansPlayerData : MonoBehaviour
{
    public bool[] landmarks;
    //index 0 - has Main Building
    //index 1 - has Tanks Factory
    public int arraySize = 2;

    [SerializeField] private FactionSpeciefcPurchasablesList factionSpeciefcPurchasablesList;
    public List<PurchasableDetails> myFactionSpeciefcPurchasablesList;
   
    private void Awake()
    {
        landmarks = new bool[arraySize];

        myFactionSpeciefcPurchasablesList = new List<PurchasableDetails>();
        print(factionSpeciefcPurchasablesList.details.Length);

        for (int i=0;i< factionSpeciefcPurchasablesList.details.Length; i++)
        {
            if (factionSpeciefcPurchasablesList.details[i] is UnitDetails)
            {
                UnitDetails tmp = (UnitDetails)ScriptableObject.CreateInstance("UnitDetails");
                tmp.CopyData((UnitDetails)factionSpeciefcPurchasablesList.details[i]);
                myFactionSpeciefcPurchasablesList.Add(tmp);
            }
            else if (factionSpeciefcPurchasablesList.details[i] is WeaponDetails)
            {
                WeaponDetails tmp = (WeaponDetails)ScriptableObject.CreateInstance("WeaponDetails");
                tmp.CopyData((WeaponDetails)factionSpeciefcPurchasablesList.details[i]);
                myFactionSpeciefcPurchasablesList.Add(tmp);
            }
            /*else
            {
                print("Purchasable!");
                factionSpeciefcPurchasablesList.details[i] = new PurchasableDetails(factionSpeciefcPurchasablesList.details[i]);
            }*/
            //foreach(var in ...)
            //myFactionSpeciefcPurchasablesList[i] = ScriptableObject.Instantiate(factionSpeciefcPurchasablesList.details[i]);
        }
    }
}

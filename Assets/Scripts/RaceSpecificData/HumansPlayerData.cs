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
            PurchasableDetails tmp = new PurchasableDetails();

            //foreach(var in ...)
            //factionSpeciefcPurchasablesList.details[i] = ScriptableObject.Instantiate(factionSpeciefcPurchasablesList.details[i]);
        }
    }
}

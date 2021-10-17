using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purchasables : MonoBehaviour
{
    public int RACE_INDEX;

    [SerializeField] public PurchasableDetails purchasableDetails;
    public float timeStartedUpgrading;
    public bool isBuilding;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual Sprite GetIcon()
    {
        return null;
    }

    public virtual int[] GetPrerequisites()
    {
        return null;
    }

    public virtual int[] GetRequirements()
    {
        return null;
    }

    public PurchasableDetails GetData()
    {
        if (this.GetComponent<Weapon>())
        {
            return this.GetComponent<Weapon>().weaponDetails;
        }

        if (this.GetComponent<Unit>())
        {
            return this.GetComponent<Unit>().unitDetails;
        }

        if (this.GetComponent<Upgrade>())
        {
            return this.GetComponent<Upgrade>().upgradeDetails;
        }

        return null;
    }

    public string GetName()
    {
        return GetData().name;
    }

    public int GetPrice()
    {
        return GetData().price;
    }

    public void Purchase(GameObject purchasingParent)
    {
        //print("Purchase " + gameObject.name + " for " + purchasingParent.gameObject.name);
        //print("My Position: " + purchasingParent.transform.position);
        if (purchasingParent.GetComponent<Unit>().myPlayer.resources>=GetPrice())
        {
            
            if (this.GetComponent<Weapon>())
            {
                purchasingParent.GetComponent<Unit>().myPlayer.AddResources(-GetPrice());
                purchasingParent.GetComponent<Purchasables>().timeStartedUpgrading = Time.time;
                PurchaseWeapon(purchasingParent);
                return;
            }

            if (this.GetComponent<Unit>())
            {
                if (this.GetComponent<Unit>().unitDetails.unitType != UnitDetails.UnitType.Building)
                {
                    purchasingParent.GetComponent<Unit>().myPlayer.AddResources(-GetPrice());
                }
                purchasingParent.GetComponent<Purchasables>().timeStartedUpgrading = Time.time;
                PurchaseUnit(purchasingParent);
                return;
            }
            if (this.GetComponent<Upgrade>())
            {
                //purchasingParent.GetComponent<Unit>().myPlayer.AddResources(-GetPrice());
                //purchasingParent.GetComponent<Purchasables>().timeStartedUpgrading = Time.time;
                PurchaseUpgrade(purchasingParent);
            }
        }
        else
        {
            print("Not enough resources");
        }
    }

    public void PurchaseUnit(GameObject purchasingParent)
    {
        int i = 0;
        foreach (Purchasables purchasable in purchasingParent.GetComponentInChildren<Unit>().GetPurchasables())
        {
            if (purchasable.name == this.gameObject.name)
            {
                purchasingParent.GetComponentInChildren<Unit>().AttemptToSpawnUnit(i);
                return;
            }
            i++;
        }
    }

    public void PurchaseWeapon(GameObject purchasingParent)
    {
        //print(purchasingParent.GetComponentInChildren<WeaponHolder>());
        if (purchasingParent.GetComponentInChildren<WeaponHolder>())
        {
            foreach(WeaponHolder weaponHolder in purchasingParent.GetComponentsInChildren<WeaponHolder>())
            {
                if (!weaponHolder.hasAWeapon)
                {
                    foreach (Purchasables purchasable in purchasingParent.GetComponentInChildren<Unit>().GetPurchasables())
                    {
                        if (purchasable.name == this.gameObject.name)
                        {
                            weaponHolder.StartBuildingUpgrade(purchasable.gameObject);
                            return;
                        }
                    }
                }
            }
        }
    }

    public void PurchaseUpgrade(GameObject purchasingParent)
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purchasables : MonoBehaviour
{
    public PurchasableDetails purchasableDetails;
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

    public void Purchase(GameObject purchasingParent)
    {
        //print("Purchase " + gameObject.name + " for " + purchasingParent.gameObject.name);
        //print("My Position: " + purchasingParent.transform.position);

        if (this.GetComponent<Weapon>())
        {
            purchasingParent.GetComponent<Purchasables>().timeStartedUpgrading = Time.time;
            PurchaseWeapon(purchasingParent);
            return;
        }

        if (this.GetComponent<Unit>())
        {
            purchasingParent.GetComponent<Purchasables>().timeStartedUpgrading = Time.time;
            PurchaseUnit(purchasingParent);
            return;
        }
    }

    public void PurchaseUnit(GameObject purchasingParent)
    {
        int i = 0;
        foreach (Purchasables purchasable in purchasingParent.GetComponentInChildren<Unit>().GetPurchasables())
        {
            if (purchasable.name == this.gameObject.name)
            {
                purchasingParent.GetComponentInChildren<Unit>().StartSpawningUnit(i);
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
}

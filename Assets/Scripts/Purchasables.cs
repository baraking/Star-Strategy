using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purchasables : MonoBehaviour
{
    public PurchasableDetails purchasableDetails;

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

    public void Purchase(GameObject purchasingParent)
    {
        //print("Purchase " + gameObject.name + " for " + purchasingParent.gameObject.name);
        //print("My Position: " + purchasingParent.transform.position);

        if (this.GetComponent<Weapon>())
        {
            PurchaseWeapon(purchasingParent);
        }

        if (this.GetComponent<Unit>())
        {
            PurchaseUnit(purchasingParent);
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
            if (!purchasingParent.GetComponentInChildren<WeaponHolder>().hasAWeapon)
            {
                foreach(Purchasables purchasable in purchasingParent.GetComponentInChildren<Unit>().GetPurchasables())
                {
                    if (purchasable.name == this.gameObject.name)
                    {
                        purchasingParent.GetComponentInChildren<WeaponHolder>().BuildUpgrade(purchasable.gameObject);
                        return;
                    }
                }
            }
        }
    }
}

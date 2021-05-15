using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Vector3 spawnPoint;
    public bool hasAWeapon;
    public List<Purchasables> possibleUpgrades;

    private void Start()
    {
        hasAWeapon = GetComponentInChildren<Weapon>();
    }

    public void BuildUpgrade(GameObject chosenPurchasable)
    {
        GameObject upgrade = Instantiate(chosenPurchasable);
        upgrade.transform.SetParent(this.transform, false);
        upgrade.transform.localPosition = spawnPoint;

        GetComponentInParent<Unit>().AddWeapons();
    }



}

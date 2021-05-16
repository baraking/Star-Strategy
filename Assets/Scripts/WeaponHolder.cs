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
        UpdateIfHasAWeapon();
    }

    public void UpdateIfHasAWeapon()
    {
        hasAWeapon = GetComponentInChildren<Weapon>();
    }

    public void StartBuildingUpgrade(GameObject chosenPurchasable)
    {
        if (!hasAWeapon)
        {
            StartCoroutine(SpawnUpgrade(chosenPurchasable));
        }
    }

    public IEnumerator SpawnUpgrade(GameObject chosenPurchasable)
    {
        hasAWeapon = true;
        yield return new WaitForSeconds(chosenPurchasable.GetComponent<Weapon>().weaponDetails.buildTime);
        GameObject upgrade = Instantiate(chosenPurchasable);
        upgrade.transform.SetParent(this.transform, false);
        upgrade.transform.localPosition = spawnPoint;

        GetComponentInParent<Unit>().AddWeapons();
    }



}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//check for units with multiple weapons
public class WeaponHolder : MonoBehaviour
{
    public Vector3 spawnPoint;
    public bool hasAWeapon;
    public List<Purchasables> possibleUpgrades;
    public float buildTime;

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
        buildTime = chosenPurchasable.GetComponent<Weapon>().weaponDetails.buildTime;
        yield return new WaitForSeconds(chosenPurchasable.GetComponent<Weapon>().weaponDetails.buildTime);
        //GameObject upgrade = Instantiate(chosenPurchasable);

        object[] instantiationData = new object[] { GetComponentInParent<Unit>().photonID };
        GameObject upgrade = PhotonNetwork.Instantiate(chosenPurchasable.name, spawnPoint, Quaternion.identity, 0,instantiationData);
        upgrade.transform.SetParent(this.transform, false);
        upgrade.transform.localPosition = spawnPoint;

        GetComponentInParent<Unit>().AddWeapons();
    }



}

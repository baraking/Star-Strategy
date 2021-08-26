using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//check for units with multiple weapons
public class WeaponHolder : MonoBehaviour
{
    public Vector3 spawnPoint;
    public bool hasAWeapon;
    public List<Purchasables> possibleUpgrades;
    public bool isBuilding;
    public float buildTime;
    public float buildProgress;

    public GameObject weapongProduced;

    private void Start()
    {
        UpdateIfHasAWeapon();
        isBuilding = false;
    }

    private void Update()
    {
        if (isBuilding)
        {
            ProduceWeapon(weapongProduced);
        }
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
        buildProgress = 0;
        hasAWeapon = true;
        buildTime = chosenPurchasable.GetComponent<Weapon>().weaponDetails.buildTime;

        GetComponentInParent<Unit>().creationQueue.Add(chosenPurchasable.GetComponent<Purchasables>());
        isBuilding = true;
        print(GetComponentInParent<Unit>().GetIsSelected());
        if (GetComponentInParent<Unit>().GetIsSelected())
        {
            GetComponentInParent<Unit>().myPlayer.DisplayPurchasableQueue(GetComponentInParent<Unit>());
        }

        //yield return new WaitForSeconds(chosenPurchasable.GetComponent<Weapon>().weaponDetails.buildTime);
        //GameObject upgrade = Instantiate(chosenPurchasable);

        yield return new WaitForSeconds(0);

        ProgressBar progressBar = GetComponentInParent<Unit>().myPlayer.playerUI.UnitCanvas.GetComponent<UnitUICanvas>().purchaseableQueueCanvas.transform.GetChild(0).GetComponentInChildren<ProgressBar>();
        progressBar.SetImageToState(true);
        progressBar.slider.maxValue = buildTime;
        progressBar.slider.value = buildProgress;

        isBuilding = true;
        weapongProduced = chosenPurchasable;
        //ProduceWeapon(chosenPurchasable);
    }

    public void ProduceWeapon(GameObject chosenPurchasable)
    {
        //print("Producing!");
        buildProgress += Time.deltaTime;
        //print("buildProgress: " + buildProgress);
        GetComponentInParent<Unit>().myPlayer.playerUI.UnitCanvas.GetComponent<UnitUICanvas>().purchaseableQueueCanvas.transform.GetChild(0).GetComponentInChildren<ProgressBar>().slider.value = buildProgress;
        //print("Add Amount: " + ((Time.deltaTime * actionTarget.GetComponent<Unit>().unitDetails.max_hp) / actionTarget.GetComponent<Unit>().unitDetails.buildTime) +"/"+ actionTarget.GetComponent<Unit>().unitDetails.max_hp);
        print(GetComponentInParent<Unit>().creationQueue[0].GetData().name + ": " + buildProgress + "/" + GetComponentInParent<Unit>().creationQueue[0].GetData().buildTime);

        if (buildProgress >= GetComponentInParent<Unit>().creationQueue[0].GetData().buildTime)
        {
            isBuilding = false;
            buildProgress = GetComponentInParent<Unit>().creationQueue[0].GetData().buildTime;
            DeployWeapon(chosenPurchasable);
            buildProgress = 0;

            Debug.Log("Finished building a " + GetComponentInParent<Unit>().creationQueue[0].GetData().name);

            GetComponentInParent<Unit>().myPlayer.playerUI.UnitCanvas.GetComponent<UnitUICanvas>().purchaseableQueueCanvas.transform.GetChild(0).GetComponentInChildren<Slider>().gameObject.SetActive(false);

            GetComponentInParent<Unit>().creationQueue.Remove(GetComponentInParent<Unit>().creationQueue[0]);
            GetComponentInParent<Unit>().myPlayer.DisplayPurchasableQueue(GetComponentInParent<Unit>());

            /*isBuilding = false;
            if (creationQueue.Count < 1)
            {
                unitAction = UnitActions.Idle;
            }
            else
            {
                actionTarget = creationQueue[0].gameObject;
                unitAction = UnitActions.Spawn;
            }*/
        }
    }

    public void DeployWeapon(GameObject chosenPurchasable)
    {
        object[] instantiationData = new object[] { GetComponentInParent<Unit>().photonID };
        GameObject upgrade = PhotonNetwork.Instantiate(chosenPurchasable.name, spawnPoint, Quaternion.identity, 0, instantiationData);
        upgrade.transform.SetParent(this.transform, false);
        upgrade.transform.localPosition = spawnPoint;

        GetComponentInParent<Unit>().AddWeapons();

        GetComponentInParent<Unit>().creationQueue.Remove(GetComponentInParent<Unit>().creationQueue[0]);
        if (GetComponentInParent<Unit>().GetIsSelected())
        {
            GetComponentInParent<Unit>().myPlayer.DisplayPurchasableQueue(GetComponentInParent<Unit>());
        }
    }



}

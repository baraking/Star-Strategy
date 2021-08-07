using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fix IsUnitSelectable(Unit other)
//may need to update/fix the player camera part.
//cameraHolder
//awake is written pretty bad
//give resources a set value
public class PlayerController : MonoBehaviourPunCallbacks
{
    public enum Race { Humans, Parasites, Bugs }
    public PhotonView photonView;

    public int playerNumber;
    public Race playingRace;
    public List<Unit> playerUnits;
    public GameObject cameraHolder;
    public Camera playerCamera;

    public bool debugMode;

    public int resources;

    public HumansPlayerData PlayerRaceData;
    public FactionStartingData factionStartingData;

    public void Awake()
    {
        //Default Setting!
        playingRace = Race.Humans;

        if (playingRace == Race.Humans)
        {
            PlayerRaceData = gameObject.AddComponent<HumansPlayerData>();
        }

        photonView = GetComponent<PhotonView>();

        playerUnits = new List<Unit>();
        GameManager.Instance.playersHolder.allPlayers.Add(this);
        //cameraHolder = gameObject.transform.FindChild("CameraHolder").gameObject;
        if (photonView.IsMine)
        {
            /*for (int i = 0; i < cameraHolder.transform.childCount; i++)
            {
                cameraHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
            playerCamera = gameObject.GetComponentInChildren<Camera>();*/
            playerCamera.gameObject.SetActive(true);

            GameManager.Instance.UnitCanvas.GetComponent<UnitUICanvas>().backgroundImage.color = GameManager.Instance.basicColors1[playerNumber-1];
        }
        
    }

    public void Start()
    {
        resources = 1000;

        /*foreach(Unit unit in playerUnits)
        {
            for(int i = 0; i < unit.GetPurchasables().Count; i++)
            {
                unit.OnUnitSpawnEnd(i);
            }
        }*/

        //factionStartingData = Resources.Load<FactionStartingData>("Assets/Units/Humans/DefaultHumans");
        //print("Has this many: " + factionStartingData.startingUnits.Length);

        if (photonView.IsMine)
        {
            SpawnStartingUnits();
        }
        SortUnits();
    }

    public void SpawnStartingUnits()
    {
        //SpawnUnitImmidiate(transform.position, factionStartingData.startingUnits[0].gameObject);
        for (int i = 0; i < factionStartingData.startingUnits.Length; i++) 
        {
            SpawnUnitImmidiate(transform.position + new Vector3(0.5f * i, 0, 0), factionStartingData.startingUnits[i].gameObject);
        }
    }

    public void SpawnUnitImmidiate(Vector3 location, GameObject purchasable)
    {
        Debug.Log("spawning a " + purchasable.GetComponent<Unit>().unitDetails.name);

        //GameObject newUnit = Instantiate(purchasable);
        object[] instantiationData = new object[] { playerNumber };
        GameObject newUnit = PhotonNetwork.Instantiate(purchasable.name, location, Quaternion.identity, 0, instantiationData);

        newUnit.GetComponent<Unit>().myPlayerNumber = playerNumber;
        newUnit.GetComponent<Unit>().myPlayer = this;
        //newUnit.transform.position = location + DEFAULT_SPAWN_LOCATION;
        newUnit.transform.position = location;
        newUnit.transform.SetParent(GameManager.Instance.Units.transform);
        newUnit.GetComponent<Unit>().healthBar = newUnit.GetComponentInChildren<HealthBar>();
        newUnit.GetComponent<Unit>().isComplete = true;
        newUnit.GetComponent<Unit>().InitUnit();
        //newUnit.GetComponent<Unit>().isComplete = false;

        //yield return new WaitForSeconds(purchasable.GetComponent<Unit>().unitDetails.buildTime);

        newUnit.GetComponent<Unit>().OnUnitSpawnEnd(purchasable);
    }

    public void SortUnits()
    {
        playerUnits.Sort();
    }

    public bool IsUnitSelectable(Unit other)
    {
        return (other.myPlayerNumber==playerNumber);
    }
}

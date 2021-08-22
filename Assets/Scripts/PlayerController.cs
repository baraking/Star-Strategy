using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

//fix IsUnitSelectable(Unit other)
//may need to update/fix the player camera part.
//cameraHolder
//awake is written pretty bad
//give resources a set value
public class PlayerController : MonoBehaviourPunCallbacks, System.IComparable
{
    public enum Race { Humans, Parasites, Bugs }
    public PhotonView photonView;

    public int playerNumber;
    public Race playingRace;
    public List<Unit> playerUnits;
    public GameObject cameraHolder;
    public Camera playerCamera;
    public PlayerUI playerUI;

    public bool debugMode;

    public int resources;

    public bool isDefeated;

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
        playerUI = GetComponent<PlayerUI>();

        playerUnits = new List<Unit>();
        GameManager.Instance.playersHolder.allPlayers.Add(this);
        GameManager.Instance.playersHolder.allPlayers.Sort();

        //cameraHolder = gameObject.transform.FindChild("CameraHolder").gameObject;
        if (photonView.IsMine)
        {
            /*for (int i = 0; i < cameraHolder.transform.childCount; i++)
            {
                cameraHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
            playerCamera = gameObject.GetComponentInChildren<Camera>();*/
            playerCamera.gameObject.SetActive(true);

            //GameManager.Instance.UnitCanvas.GetComponent<UnitUICanvas>().backgroundImage.color = GameManager.Instance.basicColors1[playerNumber-1];

            //playerUI.UnitCanvas.GetComponent<UnitUICanvas>().backgroundImage.color = GameManager.Instance.basicColors1[playerNumber];
        }

        foreach (PlayerController player in GameManager.Instance.playersHolder.allPlayers)
        {
            if (player.photonView.IsMine)
            {
                //player.playerUI.SetMovementCanvasActive();
                player.playerUI.SetMovementCanvasDeactive();
            }
            else
            {
                player.playerUI.PauseMenu.gameObject.SetActive(false);
                player.playerUI.UnitCanvas.gameObject.SetActive(false);
                player.playerUI.ExpandedMovementCanvas.gameObject.SetActive(false);
                player.playerUI.MinimizedMovementCanvas.gameObject.SetActive(false);
            }
        }

        isDefeated = false;
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

    private void Update()
    {
        /*foreach (PlayerController playerController in GameManager.Instance.playersHolder.allPlayers)
        {
            foreach (Unit unit in playerUnits)
            {
                //if (unit.GetIsSelected())
                //{
                    if (Input.GetKey(KeyCode.P))
                    {
                        photonView.RPC("RecieveCurrentAction", RpcTarget.All, unit.SendCurrentAction());
                    }
                //}
            }
        }*/
    }

    public void DisplayPurchasableQueue(Unit selectedUnit)
    {
        if (selectedUnit.creationQueue.Count < 1)
        {
            return;
        }

        foreach (Transform child in playerUI.UnitCanvas.GetComponent<UnitUICanvas>().purchaseableQueueCanvas.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int i = 0;
        foreach (Purchasables curPurchasable in selectedUnit.creationQueue)
        {
            //print("purchasables: " + curPurchasable);
            GameObject newPurchasableUI = Instantiate(GameManager.Instance.purchaseablePrefab);
            newPurchasableUI.GetComponentInChildren<Button>().image.sprite = curPurchasable.GetIcon();
            newPurchasableUI.transform.name = curPurchasable.GetName();
            newPurchasableUI.GetComponentInChildren<Button>().transform.name = i.ToString();

            newPurchasableUI.transform.SetParent(playerUI.UnitCanvas.GetComponent<UnitUICanvas>().purchaseableQueueCanvas.transform, false);

            /*if (curPurchasable.GetPrerequisites().Length > 0)
            {
                foreach (int i in curPurchasable.GetPrerequisites())
                {
                    if (!myPlayer.PlayerRaceData.landmarks[i])
                    {
                        newPurchasableUI.GetComponentInChildren<Button>().interactable = false;
                    }
                }
            }*/

            if (curPurchasable.GetComponent<Weapon>())
            {
                print("@@@@@Fix this!!@@@@@");
            }

            newPurchasableUI.GetComponentInChildren<Button>().onClick.AddListener(delegate () { selectedUnit.RemoveFromCreationQueue(Int32.Parse(newPurchasableUI.GetComponentInChildren<Button>().transform.name)); });
            print("Added for " + i);
            i++;
        }
    }

    public void UpdateUnitAction(Unit unit)
    {
        photonView.RPC("RecieveCurrentAction", RpcTarget.All, unit.SendCurrentAction());
    }

    [PunRPC]
    public void RecieveCurrentAction(int photonId, int targetPhotonId, int newUnitActionNumber, float[] quaternionData, float[] targetsPositions)
    {
        Unit actingUnit = PhotonView.Find(photonId).GetComponent<Unit>();

        if (targetPhotonId > 0)
        {
            actingUnit.actionTarget = PhotonView.Find(targetPhotonId).gameObject;
        }
        else
        {
            if (playerNumber != actingUnit.myPlayerNumber)
            {
                actingUnit.actionTarget = null;
            }
        }

        actingUnit.unitAction = UnitActions.GetUnitActionFromNumber(newUnitActionNumber);

        actingUnit.endQuaternion = new Quaternion(quaternionData[0], quaternionData[1], quaternionData[2], quaternionData[3]);

        actingUnit.targetsLocation = new List<Vector3>();
        for (int i = 0; i < targetsPositions.Length; i += 3)
        {
            actingUnit.targetsLocation.Add(new Vector3(targetsPositions[i], targetsPositions[i + 1], targetsPositions[i + 2]));
        }
        //print("Got a message: " + actingUnit + "," + actingUnit.actionTarget + "," + actingUnit.unitAction.Method.Name + "," + actingUnit.endQuaternion + "," + actingUnit.targetsLocation);
    }

    public bool CheckForDefeat(PlayerController playerController)
    {
        print("Player: " + playerController.name + " , Units: " + playerController.playerUnits.Count);
        if (playerController.playerUnits.Count < 1)
        {
            playerController.isDefeated = true;
            playerController.playerUI.DisplayDefeat();
            photonView.RPC("CheckForVictory", RpcTarget.All,playerController.playerNumber);
        }
        return playerController.isDefeated;
    }

    [PunRPC]
    public void CheckForVictory(int defeatedPlayer)
    {
        foreach (PlayerController playerController in GameManager.Instance.playersHolder.allPlayers)
        {
            if (playerController.playerNumber == defeatedPlayer)
            {
                playerController.isDefeated = true;
            }
        }

        List<PlayerController> candidateForWin = new List<PlayerController>();
        foreach (PlayerController playerController in GameManager.Instance.playersHolder.allPlayers)
        {
            if (!playerController.isDefeated)
            {
                candidateForWin.Add(playerController);
            }
        }

        if (candidateForWin.Count == 1)
        {
            candidateForWin[0].playerUI.DisplayVictory();
        }
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
        object[] instantiationData = new object[] { playerNumber, Unit.SET_TO_IS_COMPLETE };
        GameObject newUnit = PhotonNetwork.Instantiate(purchasable.name, location, Quaternion.identity, 0, instantiationData);

        newUnit.GetComponent<Unit>().myPlayerNumber = playerNumber;
        newUnit.GetComponent<Unit>().myPlayer = this;
        //newUnit.transform.position = location + DEFAULT_SPAWN_LOCATION;
        newUnit.transform.position = location;
        newUnit.transform.SetParent(GameManager.Instance.Units.transform);
        newUnit.GetComponent<Unit>().healthBar = newUnit.GetComponentInChildren<HealthBar>();
        newUnit.GetComponent<Unit>().isComplete = true;
        newUnit.GetComponent<Unit>().healthBar.DisableConstructionBar();

        newUnit.GetComponent<Unit>().InitUnit();
        //newUnit.GetComponent<Unit>().isComplete = false;

        //yield return new WaitForSeconds(purchasable.GetComponent<Unit>().unitDetails.buildTime);

        newUnit.GetComponent<Unit>().OnUnitSpawnEnd(purchasable);
    }

    public void SortUnits()
    {
        playerUnits = playerUnits.Distinct().ToList();
        playerUnits.Sort();
    }

    public bool IsUnitSelectable(Unit other)
    {
        return (other.myPlayerNumber==playerNumber);
    }

    public int CompareTo(object obj)
    {
        Unit other = obj as Unit;
        return this.name.CompareTo(other.name);
    }
}

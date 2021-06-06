using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fix IsUnitSelectable(Unit other)
//may need to update/fix the player camera part.
//cameraHolder
//awake is written pretty bad
//give resources a set value
public class Player : MonoBehaviourPunCallbacks
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

    public void Awake()
    {
        //Default Setting!
        playingRace = Race.Humans;

        if (playingRace == Race.Humans)
        {
            gameObject.AddComponent<HumansPlayerData>();
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

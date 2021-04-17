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
    public PhotonView photonView;

    public int playerNumber;
    public List<Unit> playerUnits;
    public GameObject cameraHolder;
    public Camera playerCamera;

    public int resources;

    public void Awake()
    {
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

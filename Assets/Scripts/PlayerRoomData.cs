using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerRoomData : MonoBehaviour
{

    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Dropdown PlayerFaction;
    [SerializeField] private TMP_Dropdown PlayerColor;
    [SerializeField] private TMP_Dropdown PlayerTeam;
    [SerializeField] private bool isPlayerReady;

    private int ownerId;
    public Image readyIcon;

    public static readonly string IS_PLAYER_READY = "isPlayerReady";


    //newPurchasableUI.GetComponentInChildren<Button>().onClick.AddListener(delegate () { curPurchasable.Purchase(selectedUnit.gameObject); });

    // Start is called before the first frame update
    void Start()
    {
        //int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1);
        //int index = PhotonNetwork.CurrentRoom.PlayerCount;
        //playerName.text = "Player" + "_" + index;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(int playerId)
    {
        playerName.text = "Player" + "_" + playerId;
        if (playerId != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            GetComponentInChildren<Button>().interactable = false;
        }
        //readyIcon.enabled = false;
    }

    public void SetPlayerReady(bool playerReady)
    {
        isPlayerReady = playerReady;
        readyIcon.enabled = playerReady;
    }

    public void ClickOnPlayerIsReady()
    {
        isPlayerReady = !isPlayerReady;
        //the same with the image

        //update online properties

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable()
        {
            { "isPlayerReady", isPlayerReady }
        };

        //Hashtable props = new Hashtable() { { "isPlayerReady", isPlayerReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

    }
}

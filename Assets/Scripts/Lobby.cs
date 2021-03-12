using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//MaxPlayers in Create Room
//OnJoinedRoom "SampleScene"
public class Lobby : MonoBehaviourPunCallbacks
{
    //[SerializeField] private string VersionName = "0.1";
    [SerializeField] private GameObject ConnectPanel;

    [SerializeField] private TMP_InputField CreateGameInput;
    [SerializeField] private TMP_InputField JoinGameInput;

    private void Awake()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Start()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");

        Debug.Log("Number of Players: " + PhotonNetwork.CountOfPlayers);
        Debug.Log("Number of Rooms: " + PhotonNetwork.CountOfRooms);
    }

    private void Update()
    {
        Debug.Log("Number of Players: " + PhotonNetwork.CountOfPlayers);
        Debug.Log("Number of Rooms: " + PhotonNetwork.CountOfRooms);
    }

    public void SetUsetName()
    {
        //PhotonNetwork.NickName = //PlayerName
    }

    public void CreateGame()
    {
        PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions() { MaxPlayers = 4 }, null);
    }

    public void JoinGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        //PhotonNetwork.JoinOrCreateRoom(JoinGameInput.text, roomOptions, TypedLobby.Default);
        PhotonNetwork.JoinRoom(JoinGameInput.text, null);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("SampleScene");
        Debug.Log("Joined the Room: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Room's Players Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    
}

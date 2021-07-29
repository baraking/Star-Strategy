using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//MaxPlayers in Create Room
//OnJoinedRoom "SampleScene"
public class Lobby : MonoBehaviourPunCallbacks
{
    //[SerializeField] private string VersionName = "0.1";
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject MultiPlayerPanel;
    [SerializeField] private GameObject CreateRoomPanel;
    [SerializeField] private GameObject JoinRoomPanel;


    [SerializeField] private TMP_InputField CreateGameInput;
    [SerializeField] private TMP_InputField JoinGameInput;

    [SerializeField] private TMP_Dropdown LevelSelection;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
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
        //PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions() { MaxPlayers = 4 }, null);
        PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions() { MaxPlayers = 4 }, TypedLobby.Default);
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
        print(LevelSelection.options[LevelSelection.value]);
        PhotonNetwork.LoadLevel(LevelSelection.options[LevelSelection.value].text);
        Debug.Log("Joined the Room: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Room's Players Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public void QuitGame()
    {
        //EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void ClickedOnMultiPlayerPanel()
    {
        MultiPlayerPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }

    public void ClickedOnBackFromMultiPlayerPanel()
    {
        MainMenuPanel.SetActive(true);
        MultiPlayerPanel.SetActive(false);
    }

    public void ClickedOnBackFromCreateRoom()
    {
        MultiPlayerPanel.SetActive(true);
        CreateRoomPanel.SetActive(false);
    }

    public void ClickedOnBackFromJoinRoom()
    {
        MultiPlayerPanel.SetActive(true);
        JoinRoomPanel.SetActive(false);
    }

    public void ClickedOnJoinRoom()
    {
        JoinRoomPanel.SetActive(true);
        MultiPlayerPanel.SetActive(false);
    }

    public void ClickedOnCreateRoom()
    {
        CreateRoomPanel.SetActive(true);
        MultiPlayerPanel.SetActive(false);
    }

}

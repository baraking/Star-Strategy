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
    [SerializeField] private GameObject RoomPanel;

    public Button StartGameButton;

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
        print("Are all ready? " + CheckPlayersReady());
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
        //PhotonNetwork.LoadLevel(LevelSelection.options[LevelSelection.value].text);
        Debug.Log("Joined the Room: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Room's Players Count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        RoomPanel.SetActive(true);
        JoinRoomPanel.SetActive(false);
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnCreatedRoom()
    {
        print(LevelSelection.options[LevelSelection.value]);

        //PhotonNetwork.CurrentRoom.CustomProperties
        //PhotonNetwork.CurrentRoom.CustomProperties.Add("Level_Selected", LevelSelection.options[LevelSelection.value]);
        //print(PhotonNetwork.CurrentRoom.CustomProperties);

        /*ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add("Level_Selected", LevelSelection.options[LevelSelection.value]);
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);*/

        //print("New Room Name: " + PhotonNetwork.CurrentRoom.CustomProperties["Level_Selected"]);

        /*Hashtable properties = new Hashtable { { "Level_Selected", LevelSelection.options[LevelSelection.value] } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);*/

        /*object levelSelected;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Level_Selected", out levelSelected))
        {
            print("New Room Name: " + levelSelected);
        }*/

        Debug.Log("Joined the Room: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Room's Players Count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        RoomPanel.SetActive(true);
        CreateRoomPanel.SetActive(false);
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

    public void ClickedOnBackFromRoomPanel()
    {
        MultiPlayerPanel.SetActive(true);
        RoomPanel.SetActive(false);
    }

    public void ClickedOnStartGame()
    {
        PhotonNetwork.LoadLevel(LevelSelection.options[LevelSelection.value].text);
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        print(player.NickName + " entered the room");
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {

    }

    public override void OnLeftRoom()
    {

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {

    }

    public override void OnLeftLobby()
    {

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //StartGameButton.gameObject.SetActive(CheckPlayersReady());
        StartGameButton.interactable = CheckPlayersReady();
    }

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue("isPlayerReady", out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}

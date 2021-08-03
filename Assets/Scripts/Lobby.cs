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
    public static readonly int SPACEING = 60;

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

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    public GameObject playerRoomDataPrefab;


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
        //Debug.Log("Number of Players: " + PhotonNetwork.CountOfPlayers);
        //Debug.Log("Number of Rooms: " + PhotonNetwork.CountOfRooms);
        //print("Are all ready? " + CheckPlayersReady());
    }

    public void SetUserName()
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
        Debug.Log("Who created this? " + PhotonNetwork.LocalPlayer.NickName);
        Debug.Log("Joined the Room: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Room's Players Count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        RoomPanel.SetActive(true);
        JoinRoomPanel.SetActive(false);
        //StartGameButton.gameObject.SetActive(CheckPlayersReady());
        StartGameButton.interactable = CheckPlayersReady();

        //--------------------------------------------
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        print("Attention! We have " + PhotonNetwork.PlayerList.Length + " players! They are:");

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            print(p.ActorNumber);
            GameObject entry = Instantiate(playerRoomDataPrefab);
            entry.transform.SetParent(RoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.transform.position = RoomPanel.transform.position - new Vector3(0, (p.ActorNumber - 1) * SPACEING, 0);
            //entry.GetComponent<PlayerRoomData>().Initialize(p.ActorNumber, p.NickName);
            entry.GetComponent<PlayerRoomData>().Initialize(p.ActorNumber);

            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(PlayerRoomData.IS_PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerRoomData>().SetPlayerReady((bool)isPlayerReady);
            }

            playerListEntries.Add(p.ActorNumber, entry);
        }
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

        Debug.Log("Who created this? " + PhotonNetwork.LocalPlayer.NickName);
        Debug.Log("Joined the Room: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Room's Players Count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        RoomPanel.SetActive(true);
        CreateRoomPanel.SetActive(false);

        StartGameButton.interactable = CheckPlayersReady();
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
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(LevelSelection.options[LevelSelection.value].text);
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        print(player.NickName + " entered the room");
        print("Number of Players: " + PhotonNetwork.CurrentRoom.Players.Count);
        //StartGameButton.gameObject.SetActive(CheckPlayersReady());

        //--------------------------------------------
        GameObject entry = Instantiate(playerRoomDataPrefab);
        entry.transform.SetParent(RoomPanel.transform);
        entry.transform.localScale = Vector3.one;
        entry.transform.position = RoomPanel.transform.position-new Vector3(0, (player.ActorNumber-1)*SPACEING, 0);
        entry.GetComponent<PlayerRoomData>().Initialize(player.ActorNumber);
        entry.GetComponent<PlayerRoomData>().SetPlayerReady(false);

        playerListEntries.Add(player.ActorNumber, entry);
        //--------------------------------------------

        StartGameButton.interactable = CheckPlayersReady();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //--------------------------------------------
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);
        //--------------------------------------------

        StartGameButton.interactable = CheckPlayersReady();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {

    }

    public override void OnLeftRoom()
    {
        ClickedOnBackFromRoomPanel();

        //--------------------------------------------
        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
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
        //--------------------------------------------
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(PlayerRoomData.IS_PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerRoomData>().SetPlayerReady((bool)isPlayerReady);
            }
        }
        //--------------------------------------------

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
            if (p.CustomProperties.TryGetValue(PlayerRoomData.IS_PLAYER_READY, out isPlayerReady))
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

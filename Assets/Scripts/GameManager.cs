using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject PlayerPrefab;
    public GameObject SceneCamera;
    public GameObject JoinCanvas;

    public GameObject Units;
    public GameObject newPlayer;

    public PlayersHolder playersHolder;
    public Color[] basicColors = { new Color(1, 0, 0, 1), new Color(0, 0, 1, 1), new Color(0, 1, 1, 1), new Color(0, 1, 1, 1) };

    private static GameManager _instance;

    public static int curNumberOfPlayers = 0;

    public static GameManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        SceneCamera.SetActive(false);

        //SpawnPlayer();
        JoinCanvas.SetActive(true);
    }

    private void Update()
    {
        Debug.Log("Number of Players: " + PhotonNetwork.CountOfPlayers);
        Debug.Log("Number of Rooms: " + PhotonNetwork.CountOfRooms);
        Debug.Log("Room's Players Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log("My Player'r Number: " + PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public void SpawnPlayer()
    {
        newPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0);

        newPlayer.name = "Player" + "_" + (PhotonNetwork.LocalPlayer.ActorNumber - 1);
        newPlayer.GetComponent<Player>().playerNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        newPlayer.transform.parent = playersHolder.transform;

        for (int i = 0; i < Units.transform.childCount; i++)
        {
            if (Units.transform.GetChild(i).GetComponent<Unit>().myPlayerNumber == PhotonNetwork.LocalPlayer.ActorNumber - 1)
            {
                print(newPlayer.GetComponent<Player>().playerUnits);
                newPlayer.GetComponent<Player>().playerUnits.Add(Units.transform.GetChild(i).GetComponent<Unit>());
                print(newPlayer.GetComponent<Player>().playerUnits);
            }
            Units.transform.GetChild(i).gameObject.SetActive(true);
            Units.transform.GetChild(i).GetComponent<Unit>().OnMyPlayerJoined();
        }

        curNumberOfPlayers++;
        Debug.Log("The number of players is: " + curNumberOfPlayers);

        JoinCanvas.SetActive(false);
    }
}

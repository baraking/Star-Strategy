using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject PlayerPrefab;
    public GameObject SceneCamera;

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

        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        newPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0);

        newPlayer.name = "Player" + "_" + (curNumberOfPlayers + 1);
        newPlayer.GetComponent<Player>().playerNumber = curNumberOfPlayers;
        newPlayer.transform.parent = playersHolder.transform;

        for (int i = 0; i < Units.transform.childCount; i++)
        {
            if (Units.transform.GetChild(i).GetComponent<Unit>().myPlayerNumber == curNumberOfPlayers)
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
    }
}

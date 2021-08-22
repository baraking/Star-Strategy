using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

//fix setPlayersData to find Instantiated player's prefab
//fix the basicColors1 name/colors issue
public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject PlayerPrefab;
    public GameObject SceneCamera;

    public StartPositions startPositions;

    public GameObject Units;
    public GameObject newPlayer;

    public PlayersHolder playersHolder;
    [SerializeField] public Color[] basicColors1 = { new Color(1, 0, 0, 1), new Color(0, 0, 1, 1), new Color(1, 1, 0, 1), new Color(0, 1, 0, 1) };

    private static GameManager _instance;

    public static int curNumberOfPlayers = 0;

    public GameObject purchaseablePrefab;

    public GameObject groupedUnitsPrefab;

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

        PhotonView PV = GetComponent<PhotonView>();
        SpawnPlayer();
    }

    private void Update()
    {
        /*Debug.Log("Number of Players: " + PhotonNetwork.CountOfPlayers);
        Debug.Log("Number of Rooms: " + PhotonNetwork.CountOfRooms);
        Debug.Log("Room's Players Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log("My Player'r Number: " + PhotonNetwork.LocalPlayer.ActorNumber);*/

        foreach (PlayerController playerController in playersHolder.allPlayers)
        {
            foreach (Unit unit in playerController.playerUnits)
            {
                if (unit.GetIsSelected())
                {
                    //print(unitAction.Method);
                    if (Input.GetKey(KeyCode.P))
                    {
                        //photonView.RPC("RecieveCurrentAction", RpcTarget.All, unit.SendCurrentAction());
                    }
                }
            }
        }
     }

    public void SpawnPlayer()
    {
        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1);
        object[] instantiationData = new object[] {index};

        //newPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity,0, instantiationData);
        newPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, startPositions.startPositions[index].transform.position, Quaternion.identity, 0, instantiationData);

        photonView.RPC("setPlayersData", RpcTarget.All, index);
    }

    [PunRPC]
    public void setPlayersData(int index)
    {
        Debug.Log("==================================Got a Message!!!=========================================");
        //object[] instantiationData = info.photonView.InstantiationData;

        Debug.Log("=================" + index + "=================");

        PlayerController[] tmpPlayers = GameObject.FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in tmpPlayers)
        {
            if (player.transform.parent != playersHolder)
            {
                newPlayer = player.gameObject;
                player.name = "Player" + "_" + index;
                player.GetComponent<PlayerController>().playerNumber = (int)index;
                player.playerUI.UnitCanvas.GetComponent<UnitUICanvas>().purchaseableBackgroundImage.color = GameManager.Instance.basicColors1[(int)index];
                player.playerUI.UnitCanvas.GetComponent<UnitUICanvas>().purchaseableQueueBackgroundImage.color = GameManager.Instance.basicColors1[(int)index];

                player.transform.SetParent(playersHolder.transform);
                //Debug.Log("Set parent to player");

                curNumberOfPlayers++;
                //Debug.Log("The number of players is: " + curNumberOfPlayers);
                break;
            }
        }

        GameManager.Instance.playersHolder.allPlayers.Sort();

        for (int i = 0; i < Units.transform.childCount; i++)
        {
            if (Units.transform.GetChild(i).GetComponent<Unit>().myPlayerNumber == index)
            {
                //print(newPlayer.GetComponent<Player>().playerUnits);
                newPlayer.GetComponent<PlayerController>().playerUnits.Add(Units.transform.GetChild(i).GetComponent<Unit>());
                //print(newPlayer.GetComponent<Player>().playerUnits);
            }
            newPlayer.GetComponent<PlayerController>().SortUnits();
            Units.transform.GetChild(i).gameObject.SetActive(true);
            Units.transform.GetChild(i).GetComponent<Unit>().OnMyPlayerJoined();
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;

        Debug.Log("================="+ instantiationData[0]+ "=================");

        GameObject[] tmpPlayers = (GameObject[])GameObject.FindObjectsOfType(typeof(PlayerController));
        foreach (GameObject player in tmpPlayers)
        {
            if (player.transform.parent != playersHolder)
            {
                newPlayer = player;
                player.name = "Player" + "_" + instantiationData[0];
                player.GetComponent<PlayerController>().playerNumber = (int)instantiationData[0];
                player.transform.SetParent(playersHolder.transform);
                Debug.Log("Set parent to player");

                curNumberOfPlayers++;
                Debug.Log("The number of players is: " + curNumberOfPlayers);
                return;
            }
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        //EditorApplication.isPlaying = false;
        Application.Quit();
    }
}

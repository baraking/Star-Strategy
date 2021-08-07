using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fix setPlayersData to find Instantiated player's prefab
//fix the basicColors1 name/colors issue
public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject PlayerPrefab;
    public GameObject SceneCamera;
    public GameObject UnitCanvas;
    public GameObject ExpandedMovementCanvas;
    public GameObject MinimizedMovementCanvas;

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
        UnitCanvas.SetActive(false);

        PhotonView PV = GetComponent<PhotonView>();
        SpawnPlayer();
    }

    public void SetUnitCanvasActive()
    {
        UnitCanvas.SetActive(true);
    }

    public void SetUnitCanvasDeactive()
    {
        UnitCanvas.SetActive(false);
    }

    public void SetMovementCanvasActive()
    {
        ExpandedMovementCanvas.SetActive(true);
        MinimizedMovementCanvas.SetActive(false);
    }

    public void SetMovementCanvasDeactive()
    {
        MinimizedMovementCanvas.SetActive(true);
        ExpandedMovementCanvas.SetActive(false);
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
                        //Die();

                        //photonView.RPC("RecieveCurrentAction", RpcTarget.All, unit.SendCurrentAction());

                        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1);
                        object[] instantiationData = new object[] { index };

                        //photonView.RPC("sendGoodbye", RpcTarget.All, instantiationData);

                        object[] instantiationData2 = new object[] { index, index+3 };

                        //photonView.RPC("sendLater", RpcTarget.All, instantiationData2);

                        object[] instantiationData3 = new object[] { unit.photonID, index, new int[] { 1, 2, 3, 4 }};

                        //photonView.RPC("sendTest", RpcTarget.All, instantiationData3);

                        object[] instantiationData4 = new object[] { unit.photonID, index, new int[] { 1, 2, 3, 4 }, new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } } };

                        photonView.RPC("sendGoodbye", RpcTarget.All, index);

                        photonView.RPC("sendFinalTest", RpcTarget.All, new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } });
                    }
                }
            }
        }
     }

    [PunRPC]
    public void sendHello()
    {
        print("Hello!!");
    }

    [PunRPC]
    public void sendGoodbye(int message)
    {
        print("Goodbye!!");
    }

    [PunRPC]
    public void sendLater(int message1, int message2)
    {
        print(message1 + " , " + message2);
    }

    [PunRPC]
    public void sendTest(int message1, int message2, int[] data1)
    {
        print("Test!!");
    }

    [PunRPC]
    public void sendFinalTest(int[,] data2)
    {
        print("Test!!");
    }

    [PunRPC]
    public void RecieveCurrentAction(object[] message)
    {
        Unit actingUnit = PhotonView.Find((int)message[0]).GetComponent<Unit>();
        actingUnit.actionTarget = PhotonView.Find((int)message[1]).gameObject;
        actingUnit.endQuaternion = new Quaternion((int)((object[])message[2])[0], (int)((object[])message[2])[1], (int)((object[])message[2])[2], (int)((object[])message[2])[3]);
        actingUnit.targetsLocation = new List<Vector3>();
        for (int i = 0; i < ((object[])message[3]).Length; i++)
        {
            actingUnit.targetsLocation.Add(new Vector3((int)((object[])message[3])[0], (int)((object[])message[3])[1], (int)((object[])message[3])[2]));
        }
        print(actingUnit + "," + actingUnit.actionTarget + "," + actingUnit.endQuaternion + "," + actingUnit.targetsLocation);
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
                player.transform.SetParent(playersHolder.transform);
                Debug.Log("Set parent to player");

                curNumberOfPlayers++;
                Debug.Log("The number of players is: " + curNumberOfPlayers);
                break;
            }
        }

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
}

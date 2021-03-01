using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayersHolder playersHolder;
    public Color[] basicColors = { new Color(1, 0, 0, 1), new Color(0, 0, 1, 1), new Color(0, 1, 1, 1), new Color(0, 1, 1, 1) };

    private static GameManager _instance;

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
    }
}

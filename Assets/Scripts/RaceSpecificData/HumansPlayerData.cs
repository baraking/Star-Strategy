using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumansPlayerData : MonoBehaviour
{
    public bool[] landmarks;
    //index 0 - has Main Building
    //index 1 - has Tanks Factory
    public int arraySize = 2;

    private void Awake()
    {
        landmarks = new bool[arraySize];
    }
}

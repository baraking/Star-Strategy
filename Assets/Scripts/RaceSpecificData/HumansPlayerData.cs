using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumansPlayerData : MonoBehaviour
{
    public bool[] landmarks;
    //index 0 - has Tanks Factory
    public int arraySize = 1;

    private void Awake()
    {
        landmarks = new bool[arraySize];
    }
}

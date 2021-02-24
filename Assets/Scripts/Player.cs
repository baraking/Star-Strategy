using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fix IsUnitSelectable(Unit other)
public class Player : MonoBehaviour
{
    public int playerNumber;
    public List<Unit> playerUnits;

    public void Awake()
    {
        playerUnits = new List<Unit>();
    }

    public bool IsUnitSelectable(Unit other)
    {
        return (other.myPlayerNumber==playerNumber);
    }
}

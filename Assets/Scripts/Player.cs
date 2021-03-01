using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fix IsUnitSelectable(Unit other)
//may need to update/fix the player camera part.
public class Player : MonoBehaviour
{
    public int playerNumber;
    public List<Unit> playerUnits;
    public Camera playerCamera;

    public void Awake()
    {
        playerUnits = new List<Unit>();
        playerCamera = gameObject.GetComponentInChildren<Camera>();
    }

    public bool IsUnitSelectable(Unit other)
    {
        return (other.myPlayerNumber==playerNumber);
    }
}

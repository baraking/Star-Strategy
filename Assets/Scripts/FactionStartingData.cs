using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FactionStartingData", menuName = "Faction Starting Data")]
public class FactionStartingData : ScriptableObject
{
    public string factionStartingDataName;
    public PlayerController.Race race;
    public Unit[] startingUnits;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//need to automatically add and handle all players.
public class PlayersHolder : MonoBehaviour
{
    public List<Player> allPlayers;

    public Player getPlayer(int playerNumber)
    {
        foreach (Player player in allPlayers)
        {
            if (player.playerNumber == playerNumber)
            {
                return player;
            }
        }
        Debug.LogError("Player number was not found!");
        return null;
    }
}

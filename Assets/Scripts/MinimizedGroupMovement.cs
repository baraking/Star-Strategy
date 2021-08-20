using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimizedGroupMovement : MonoBehaviour
{
    public GameObject maximizedGroupMovement;

    public Image smallChosenIcon;

    public Sprite smallArcDefensiveFormation;
    public Sprite smallArcOffensiveFormation;
    public Sprite smallCircleFormation;
    public Sprite smallRowFormation;
    public Sprite smallPointFormation;

    public void closeMovementCanvas()
    {
        foreach(PlayerController player in GameManager.Instance.playersHolder.allPlayers)
        {
            if (player.photonView.IsMine)
            {
                player.playerUI.SetMovementCanvasDeactive();
                return;
            }
        }
    }

    public void openMovementCanvas()
    {
        foreach (PlayerController player in GameManager.Instance.playersHolder.allPlayers)
        {
            if (player.photonView.IsMine)
            {
                player.playerUI.SetMovementCanvasActive();
                return;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    public GameObject UnitCanvas;
    public GameObject PurchaseableQueueCanvas;
    public GameObject PauseMenu;
    public GameObject ExpandedMovementCanvas;
    public GameObject MinimizedMovementCanvas;
    public GameObject EndGameMessage;
    public GameObject VictoryMessage;
    public GameObject DefeatMessage;
    public GameObject ResourcesPanel;
    public TMP_Text ResourcesAmount;

    private void Awake()
    {
        UnitCanvas.SetActive(false);
        PauseMenu.SetActive(false);
    }

    public void DisplayVictory()
    {
        EndGameMessage.SetActive(true);
        DefeatMessage.SetActive(false);
        VictoryMessage.SetActive(true);
    }

    public void DisplayDefeat()
    {
        EndGameMessage.SetActive(true);
        VictoryMessage.SetActive(false);
        DefeatMessage.SetActive(true);
    }

    public void SetUnitCanvasActive()
    {
        UnitCanvas.SetActive(true);
    }

    public void SetUnitCanvasDeactive()
    {
        UnitCanvas.SetActive(false);
    }

    public void SetPauseMenusActive()
    {
        PauseMenu.SetActive(true);
    }

    public void SetPauseMenusDeactive()
    {
        PauseMenu.SetActive(false);
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

    public void ContinueGame()
    {
        SetPauseMenusDeactive();
    }

    public void OpenPauseMenu()
    {
        SetPauseMenusActive();
    }

    public void CallBackToMainMenuFromGameManager()
    {
        GameManager.Instance.BackToMainMenu();
    }

    public void CallQuitGameFromGameManager()
    {
        GameManager.Instance.QuitGame();
    }
}

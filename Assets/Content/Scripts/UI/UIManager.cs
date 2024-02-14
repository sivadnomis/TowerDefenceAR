using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject startButton;
    public GameObject endGamePopup;
    public GameObject placeTurretButton;
    public GameObject creditBalancePanel;

    public void OnGameManagerSpawned()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameEndedEvent.AddListener(OnEndGame);
        }
        else
        {
            Debug.LogWarning("GameManager not found");
        }

        EnableStartUI();
    }

    public void EnableStartUI()
    {
        startButton.SetActive(true);
    }

    public void StartGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.StartGame();
            startButton.SetActive(false);
            placeTurretButton.SetActive(true);
            creditBalancePanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RestartGame();
            endGamePopup.SetActive(false);
            placeTurretButton.SetActive(true);
        }
    }

    public void OnTurretIconSelected(GameObject turretPrefab)
    {
        GameManager.instance.GetTurretPlacementController().OnTurretIconSelected(turretPrefab);
    }

    public void OnEndGame()
    {
        placeTurretButton.SetActive(false);
        endGamePopup.SetActive(true);
    }

    private void OnDestroy()
    {
        GameManager.instance.gameEndedEvent.RemoveListener(OnEndGame);
    }
}

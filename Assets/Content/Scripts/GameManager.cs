using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public enum GameState
    {
        Setup,
        Playing,
        End
    };

    public float gravity;

    private GameState gameState;
    public UnityEvent gameStartedEvent;
    public UnityEvent gameEndedEvent;
    public UnityEvent gameRestartedEvent;

    public TurretPlacementController turretPlacementController;

    private void Start()
    {
        Physics.gravity = Vector3.up * gravity;

        gameState = GameState.Setup;

        if (UIManager.instance != null)
        {
            UIManager.instance.OnGameManagerSpawned();
        }

        if (BankManager.instance != null)
        {
            BankManager.instance.OnGameManagerSpawned();
        }
    }

    public void StartGame()
    {
        SetGameState(GameState.Playing);
        gameStartedEvent.Invoke();
    }

    public void EndGame()
    {
        SetGameState(GameState.End);
        gameEndedEvent.Invoke();
    }

    public void RestartGame()
    {
        gameRestartedEvent.Invoke();
        StartGame();
    }

    private void SetGameState(GameState newState)
    {
        gameState = newState;
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public float GetGravity()
    {
        return gravity;
    }

    public TurretPlacementController GetTurretPlacementController()
    {
        if (turretPlacementController == null)
        {
            Debug.LogWarning("No TurretPlacementController found");
            return null;
        }

        return turretPlacementController;
    }

    public TurretPlacementController.TurretPlacementState GetTurretPlacementState()
    {
        return turretPlacementController.GetTurretPlacementState();
    }

    public void RegisterEnemyDeath()
    {
        if (EnemySpawner.instance.GetNumEnemiesLeftToSpawn() <= 0 && AllEnemiesAreDead())
        {
            EndGame();
        }
    }

    private bool AllEnemiesAreDead()
    {
        bool allEnemiesDead = true;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<IMortalUnit>().GetCurrentState() != MortalUnitStateMachine.MortalUnitState.Die)
            {
                allEnemiesDead = false;
            }
        }

        return allEnemiesDead;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region Singleton
    public static EnemySpawner instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    private ObjectPooler objectPooler;
    private Coroutine spawnUnits;
    private int numEnemiesLeftToSpawn;

    public string poolTag = "Enemy";
    public int numEnemiesToSpawn;
    public float spawnInterval;
    public float spawnHeightFromGround;

    private void Start()
    {
        objectPooler = ObjectPooler.instance;
        if (GameManager.instance != null)
        {
            GameManager.instance.gameStartedEvent.AddListener(OnGameStart);
            GameManager.instance.gameEndedEvent.AddListener(OnGameEnd);
        }
    }

    private void OnGameStart()
    {
        numEnemiesLeftToSpawn = numEnemiesToSpawn;
        spawnUnits = StartCoroutine(SpawnUnits());
    }

    private IEnumerator SpawnUnits()
    {
        while (numEnemiesLeftToSpawn > 0 && GameManager.instance.GetGameState() == GameManager.GameState.Playing)
        {
            yield return new WaitForSeconds(spawnInterval);

            Vector3 spawnPos = RandomSpawnPositionGenerator.instance.GetRandomPointOnCircleAroundTower(spawnHeightFromGround);

            if (objectPooler.SpawnFromPool(poolTag, spawnPos, Quaternion.identity) != null)
            {
                numEnemiesLeftToSpawn--;
            }
        }
    }

    public int GetNumEnemiesLeftToSpawn()
    {
        return numEnemiesLeftToSpawn;
    }

    private void OnGameEnd()
    {
        if (spawnUnits != null)
        {
            StopCoroutine(spawnUnits);
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.gameStartedEvent.RemoveListener(OnGameStart);
        GameManager.instance.gameEndedEvent.RemoveListener(OnGameEnd);
    }
}

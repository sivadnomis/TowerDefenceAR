using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnPositionGenerator : MonoBehaviour
{
    #region Singleton
    public static RandomSpawnPositionGenerator instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public Transform towerPos;
    public float spawnDistanceFromTower;

    public Vector3 GetRandomPointOnCircleAroundTower(float spawnHeightOffset)
    {
        float ang = Random.value * 360;
        Vector3 pos;

        pos.x = towerPos.position.x + spawnDistanceFromTower * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = towerPos.position.y + spawnHeightOffset;
        pos.z = towerPos.position.z + spawnDistanceFromTower * Mathf.Cos(ang * Mathf.Deg2Rad);

        return pos;
    }
}

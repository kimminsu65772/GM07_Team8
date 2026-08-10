using System;
using UnityEngine;

[Serializable]
public class EnemySpawnEntry
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField, Min(1)] private int spawnCount = 1;
    [SerializeField, Min(0f)] private float spawnInterval = 1f;

    public GameObject EnemyPrefab => enemyPrefab;
    public int SpawnCount => spawnCount;
    public float SpawnInterval => spawnInterval;
}
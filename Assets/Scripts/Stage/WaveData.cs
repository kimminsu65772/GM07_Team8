using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewWaveData",
    menuName = "Game Data/Stage/Wave Data"
)]
public class WaveData : ScriptableObject
{
    [Header("Spawn")]

    // 적 프리팹, 생성 수, 생성 간격 등의 정보가 들어간다.
    [SerializeField]
    private List<EnemySpawnEntry> enemySpawns =
        new List<EnemySpawnEntry>();

    // 다음 웨이브까지 기다리는 시간
    [SerializeField, Min(0f)]
    private float nextWaveDelay = 2f;

    [Header("Boss")]

    // 현재 웨이브가 보스 웨이브인지 구분한다.
    [SerializeField]
    private bool isBossWave;

    // 보스 웨이브의 제한시간
    [SerializeField, Min(0f)]
    private float timeLimit;

    public IReadOnlyList<EnemySpawnEntry> EnemySpawns =>
        enemySpawns;

    public float NextWaveDelay => nextWaveDelay;
    public bool IsBossWave => isBossWave;
    public float TimeLimit => timeLimit;
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTestManager : MonoBehaviour
{
    public event Action OnEnemyKilled;
    public event Action<int> OnStageCompleted;
    public event Action<int, string> OnStageFailed;

    [Header("Stage")]
    [SerializeField] private StageCatalog stageCatalog;
    public int LastStage => stageCatalog == null ? 0 : stageCatalog.StageCount;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform target;

    [Header("Runtime Information")]
    [SerializeField] private int currentStageNumber;
    [SerializeField] private int currentWaveIndex;
    [SerializeField] private int aliveEnemyCount;
    [SerializeField] private float remainingBossTime;
    [SerializeField] private bool isBossTimeOver;
    [SerializeField] private bool isAirshipDestroyed;
    [SerializeField] private bool isStageFinished;

    private readonly List<EnemyStats> trackedEnemies =
        new List<EnemyStats>();

    private Coroutine stageRoutine;

    public void StartStage(int stageNumber)
    {
        StageData stageData = FindStageData(stageNumber);

        if (stageData == null)
        {
            Debug.LogError(
                $"StageTestManager: Stage {stageNumber}에 해당하는 StageData가 없습니다."
            );
            return;
        }

        StartStage(stageData);
    }

    private void StartStage(StageData stageData)
    {
        if (stageData == null)
        {
            Debug.LogError("StageTestManager: StageData가 없습니다.");
            return;
        }

        if (spawnPoint == null || target == null)
        {
            Debug.LogError(
                "StageTestManager: SpawnPoint 또는 Target이 연결되지 않았습니다."
            );
            return;
        }

        StopStage();
        currentStageNumber = stageData.StageNumber;
        stageRoutine = StartCoroutine(RunStage(stageData));
    }

    public void StopStage()
    {
        if (stageRoutine != null)
        {
            StopCoroutine(stageRoutine);
            stageRoutine = null;
        }

        ClearTrackedEnemies();
        ResetRuntimeState();
    }

    private StageData FindStageData(int stageNumber)
    {
        if (stageCatalog == null)
        {
            Debug.LogError("StageTestManager: StageCatalog가 연결되지 않았습니다.");
            return null;
        }

        stageCatalog.TryGetStageData(stageNumber, out StageData stageData);
        return stageData;
    }

    private IEnumerator RunStage(StageData stageData)
    {
        ResetRuntimeState();
        currentStageNumber = stageData.StageNumber;

        for (int i = 0; i < stageData.Waves.Count; i++)
        {
            currentWaveIndex = i;
            isBossTimeOver = false;

            WaveData currentWave = stageData.Waves[i];

            yield return StartCoroutine(
                SpawnWave(currentWave)
            );

            if (isAirshipDestroyed || target == null)
            {
                FailStage("비행선 파괴: 스테이지 실패");
                yield break;
            }

            yield return StartCoroutine(
                WaitForWaveClear(currentWave)
            );

            if (isAirshipDestroyed)
            {
                FailStage("비행선 파괴: 스테이지 실패");
                yield break;
            }

            if (isBossTimeOver)
            {
                FailStage("보스 제한시간 초과: 스테이지 실패");
                yield break;
            }

            bool hasNextWave = i < stageData.Waves.Count - 1;

            if (hasNextWave &&
                currentWave.NextWaveDelay > 0f)
            {
                yield return StartCoroutine(
                    WaitForDelay(currentWave.NextWaveDelay)
                );

                if (isAirshipDestroyed)
                {
                    FailStage("비행선 파괴: 스테이지 실패");
                    yield break;
                }
            }
        }

        CompleteStage();
    }

    private IEnumerator SpawnWave(WaveData waveData)
    {
        foreach (EnemySpawnEntry spawnEntry in waveData.EnemySpawns)
        {
            if (spawnEntry.EnemyPrefab == null)
            {
                Debug.LogWarning(
                    $"{waveData.name}에 Enemy Prefab이 없습니다."
                );
                continue;
            }

            for (int i = 0; i < spawnEntry.SpawnCount; i++)
            {
                if (target == null)
                {
                    isAirshipDestroyed = true;
                    yield break;
                }

                SpawnEnemy(spawnEntry.EnemyPrefab);

                bool hasNextEnemy = i < spawnEntry.SpawnCount - 1;

                if (hasNextEnemy &&
                    spawnEntry.SpawnInterval > 0f)
                {
                    yield return StartCoroutine(
                        WaitForDelay(spawnEntry.SpawnInterval)
                    );

                    if (isAirshipDestroyed)
                    {
                        yield break;
                    }
                }
            }
        }
    }

    private IEnumerator WaitForWaveClear(WaveData waveData)
    {
        if (!waveData.IsBossWave ||
            waveData.TimeLimit <= 0f)
        {
            remainingBossTime = 0f;

            while (true)
            {
                RefreshAliveEnemyCount();

                if (aliveEnemyCount <= 0 ||
                    target == null)
                {
                    break;
                }

                yield return null;
            }

            if (target == null)
            {
                isAirshipDestroyed = true;
            }

            yield break;
        }

        remainingBossTime = waveData.TimeLimit;

        while (true)
        {
            RefreshAliveEnemyCount();

            if (aliveEnemyCount <= 0 ||
                remainingBossTime <= 0f ||
                target == null)
            {
                break;
            }

            remainingBossTime -= Time.deltaTime;
            yield return null;
        }

        remainingBossTime = Mathf.Max(remainingBossTime, 0f);

        if (target == null)
        {
            isAirshipDestroyed = true;
            yield break;
        }

        if (aliveEnemyCount > 0)
        {
            isBossTimeOver = true;
        }
    }

    private IEnumerator WaitForDelay(float delay)
    {
        float elapsedTime = 0f;

        while (elapsedTime < delay)
        {
            if (target == null)
            {
                isAirshipDestroyed = true;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        GameObject spawnedEnemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        EnemyStats enemyStats = spawnedEnemy.GetComponent<EnemyStats>();

        if (enemyStats == null)
        {
            Debug.LogError(
                $"{spawnedEnemy.name}에 EnemyStats가 없습니다."
            );
            Destroy(spawnedEnemy);
            return;
        }

        EnemyMovement enemyMovement =
            spawnedEnemy.GetComponent<EnemyMovement>();

        if (enemyMovement != null)
        {
            enemyMovement.SetTarget(target);
        }

        EnemyAttack enemyAttack =
            spawnedEnemy.GetComponent<EnemyAttack>();

        if (enemyAttack != null)
        {
            enemyAttack.SetTarget(target);
        }

        DummyEnemyAttack dummyEnemyAttack =
            spawnedEnemy.GetComponent<DummyEnemyAttack>();

        if (dummyEnemyAttack != null)
        {
            dummyEnemyAttack.SetTarget(target);
        }

        trackedEnemies.Add(enemyStats);
        aliveEnemyCount = trackedEnemies.Count;

        enemyStats.EnemyDied += HandleEnemyDied;
    }

    private void HandleEnemyDied(EnemyStats deadEnemy)
    {
        if (deadEnemy == null)
        {
            return;
        }

        deadEnemy.EnemyDied -= HandleEnemyDied;
        trackedEnemies.Remove(deadEnemy);
        aliveEnemyCount = trackedEnemies.Count;

        OnEnemyKilled?.Invoke();
    }

    private void RefreshAliveEnemyCount()
    {
        for (int i = trackedEnemies.Count - 1; i >= 0; i--)
        {
            if (trackedEnemies[i] == null)
            {
                trackedEnemies.RemoveAt(i);
            }
        }

        aliveEnemyCount = trackedEnemies.Count;
    }

    private void ResetRuntimeState()
    {
        currentWaveIndex = 0;
        aliveEnemyCount = 0;
        remainingBossTime = 0f;
        isBossTimeOver = false;
        isAirshipDestroyed = false;
        isStageFinished = false;
    }

    private void ClearTrackedEnemies()
    {
        foreach (EnemyStats enemy in trackedEnemies)
        {
            if (enemy == null)
            {
                continue;
            }

            enemy.EnemyDied -= HandleEnemyDied;
            Destroy(enemy.gameObject);
        }

        trackedEnemies.Clear();
    }

    private void CompleteStage()
    {
        if (isStageFinished)
        {
            return;
        }

        isStageFinished = true;
        stageRoutine = null;

        Debug.Log(
            $"Stage {currentStageNumber}의 모든 웨이브가 완료되었습니다."
        );

        OnStageCompleted?.Invoke(currentStageNumber);
    }

    private void FailStage(string failureMessage)
    {
        if (isStageFinished)
        {
            return;
        }

        isStageFinished = true;
        stageRoutine = null;

        Debug.Log(failureMessage);

        OnStageFailed?.Invoke(currentStageNumber, failureMessage);
    }

    private void OnDisable()
    {
        StopStage();
    }
}

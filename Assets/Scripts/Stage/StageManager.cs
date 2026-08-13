using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // 외부 시스템에서 스테이지 진행 상황을 받을 수 있는 이벤트
    public event Action OnEnemyKilled;
    public event Action<int> OnStageCompleted;
    public event Action<int, string> OnStageFailed;

    [Header("Stage")]
    [SerializeField] private StageCatalog stageCatalog;
    [SerializeField] private float spawnYOffset = 1.6f;

    public int LastStage =>
        stageCatalog == null
            ? 0
            : stageCatalog.StageCount;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    // 실제 비행선 또는 비행선 공격 위치
    [SerializeField] private Transform target;

    [Header("Runtime Information")]
    [SerializeField] private int currentStageNumber;
    [SerializeField] private int currentWaveIndex;
    [SerializeField] private int aliveEnemyCount;
    [SerializeField] private int remainingEnemyCount;
    [SerializeField] private float remainingBossTime;
    [SerializeField] private bool isBossTimeOver;
    [SerializeField] private bool isAirshipDestroyed;
    [SerializeField] private bool isStageFinished;

    // 생성한 적 추적
    private readonly List<EnemyStats> trackedEnemies =
        new List<EnemyStats>();

    private Coroutine stageRoutine;
    private void Start()
    {
        if (stageCatalog == null)
        {
            Debug.LogError(
                "StageManager: StageCatalog가 연결되지 않았습니다."
            );

            return;
        }

    }
   

    public void StartStage(int stageNumber)
    {
        StageData stageData =
            FindStageData(stageNumber);

        if (stageData == null)
        {
            Debug.LogError(
                $"StageManager: Stage {stageNumber}에 해당하는 StageData가 없습니다."
            );

            return;
        }

        StartStage(stageData);
    }

    private void StartStage(StageData stageData)
    {
        if (stageData == null)
        {
            Debug.LogError(
                "StageManager: StageData가 없습니다."
            );

            return;
        }

        if (spawnPoint == null ||
            target == null)
        {
            Debug.LogError(
                "StageManager: SpawnPoint 또는 Target이 연결되지 않았습니다."
            );

            return;
        }

        StopStage();

        currentStageNumber =
            stageData.StageNumber;

        stageRoutine =
            StartCoroutine(
                RunStage(stageData)
            );
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

    private StageData FindStageData(
        int stageNumber)
    {
        if (stageCatalog == null)
        {
            Debug.LogError(
                "StageManager: StageCatalog가 연결되지 않았습니다."
            );

            return null;
        }

        stageCatalog.TryGetStageData(
            stageNumber,
            out StageData stageData
        );

        return stageData;
    }

    private void SpawnEnemy(
     GameObject enemyPrefab,
     int spawnIndex)
    {
        // 위, 가운데, 아래 순서로 위치를 나눠서 생성
        float yOffset =
            (spawnIndex % 3 - 1) * spawnYOffset;

        Vector3 spawnPosition =
            spawnPoint.position +
            new Vector3(
                0f,
                yOffset,
                0f
            );

        GameObject spawnedEnemy =
            Instantiate(
                enemyPrefab,
                spawnPosition,
                Quaternion.identity
            );

        EnemyStats enemyStats =
            spawnedEnemy.GetComponent<EnemyStats>();

        if (enemyStats == null)
        {
            Debug.LogError(
                $"{spawnedEnemy.name}에 EnemyStats가 없습니다."
            );

            Destroy(spawnedEnemy);
            return;
        }

        aliveEnemyCount++;
        remainingEnemyCount++;

        trackedEnemies.Add(enemyStats);

        enemyStats.EnemyDied +=
            HandleEnemyDied;

        enemyStats.EnemyDeathCompleted +=
            HandleEnemyDeathCompleted;

        // 적한테 비행선 위치 전달
        EnemyTargetSelector targetSelector =
            spawnedEnemy.GetComponent<EnemyTargetSelector>();

        if (targetSelector != null)
        {
            targetSelector.SetAirshipTarget(target);
        }
        else
        {
            Debug.LogWarning(
                $"{spawnedEnemy.name}에 EnemyTargetSelector가 없습니다."
            );
        }
    }
    private IEnumerator RunStage(
        StageData stageData)
    {
        ResetRuntimeState();

        currentStageNumber =
            stageData.StageNumber;

        for (int i = 0;
             i < stageData.Waves.Count;
             i++)
        {
            currentWaveIndex = i;
            isBossTimeOver = false;

            WaveData currentWave =
                stageData.Waves[i];

            // 웨이브 적 생성
            yield return StartCoroutine(
                SpawnWave(currentWave)
            );

            // 적 생성 중 비행선 HP가 0이 됐다면 실패
            if (isAirshipDestroyed)
            {
                FailStage(
                    "비행선 파괴: 스테이지 실패"
                );

                yield break;
            }

            // 웨이브 종료 대기
            yield return StartCoroutine(
                WaitForWaveClear(currentWave)
            );

            if (isAirshipDestroyed)
            {
                FailStage(
                    "비행선 파괴: 스테이지 실패"
                );

                yield break;
            }

            if (isBossTimeOver)
            {
                FailStage(
                    "보스 제한시간 초과: 스테이지 실패"
                );

                yield break;
            }

            bool hasNextWave =
                i < stageData.Waves.Count - 1;

            if (hasNextWave &&
                currentWave.NextWaveDelay > 0f)
            {
                yield return StartCoroutine(
                    WaitForDelay(
                        currentWave.NextWaveDelay
                    )
                );

                if (isAirshipDestroyed)
                {
                    FailStage(
                        "비행선 파괴: 스테이지 실패"
                    );

                    yield break;
                }
            }
        }

        CompleteStage();
    }

   

    private IEnumerator SpawnWave(
        WaveData waveData)
    {
        foreach (EnemySpawnEntry spawnEntry
                 in waveData.EnemySpawns)
        {
            if (spawnEntry.EnemyPrefab == null)
            {
                Debug.LogWarning(
                    $"{waveData.name}에 Enemy Prefab이 없습니다."
                );

                continue;
            }

            for (int i = 0;
                 i < spawnEntry.SpawnCount;
                 i++)
            {
                // 비행선이 파괴되었다면 추가 생성 중단
                if (isAirshipDestroyed)
                {
                    yield break;
                }

                SpawnEnemy(
                      spawnEntry.EnemyPrefab,
                      i
                        );
                bool hasNextEnemy =
                    i < spawnEntry.SpawnCount - 1;

                if (hasNextEnemy &&
                    spawnEntry.SpawnInterval > 0f)
                {
                    yield return StartCoroutine(
                        WaitForDelay(
                            spawnEntry.SpawnInterval
                        )
                    );

                    if (isAirshipDestroyed)
                    {
                        yield break;
                    }
                }
            }
        }
    }

  

    private IEnumerator WaitForWaveClear(
        WaveData waveData)
    {
        // 일반 웨이브
        if (!waveData.IsBossWave ||
            waveData.TimeLimit <= 0f)
        {
            remainingBossTime = 0f;

            while (true)
            {
                RefreshAliveEnemyCount();

                // 모든 적의 사망 모션 / 제거까지 완료
                // 또는 비행선 파괴
                if (remainingEnemyCount <= 0 ||
                    isAirshipDestroyed)
                {
                    break;
                }

                yield return null;
            }

            yield break;
        }

      

        remainingBossTime =
            waveData.TimeLimit;

        while (true)
        {
            RefreshAliveEnemyCount();

            if (aliveEnemyCount <= 0 ||
                remainingBossTime <= 0f ||
                isAirshipDestroyed)
            {
                break;
            }

            remainingBossTime -=
                Time.deltaTime;

            yield return null;
        }

        remainingBossTime =
            Mathf.Max(
                remainingBossTime,
                0f
            );

        // 비행선이 먼저 파괴된 경우
        if (isAirshipDestroyed)
        {
            yield break;
        }

        // 제한 시간이 끝났는데 보스가 살아있음
        if (aliveEnemyCount > 0)
        {
            isBossTimeOver = true;
            yield break;
        }

        // 보스 HP는 0이되고 시체 제거가 끝날 때까지 기다림
        while (remainingEnemyCount > 0 &&
               !isAirshipDestroyed)
        {
            RefreshAliveEnemyCount();

            yield return null;
        }
    }

   

    private IEnumerator WaitForDelay(
        float delay)
    {
        float elapsedTime = 0f;

        while (elapsedTime < delay)
        {
            // 대기 중 비행선이 파괴되면 즉시 중단
            if (isAirshipDestroyed)
            {
                yield break;
            }

            elapsedTime +=
                Time.deltaTime;

            yield return null;
        }
    }

    

    private void SpawnEnemy(
        GameObject enemyPrefab)
    {
        GameObject spawnedEnemy =
            Instantiate(
                enemyPrefab,
                spawnPoint.position,
                Quaternion.identity
            );

        EnemyStats enemyStats =
            spawnedEnemy.GetComponent<EnemyStats>();

        if (enemyStats == null)
        {
            Debug.LogError(
                $"{spawnedEnemy.name}에 EnemyStats가 없습니다."
            );

            Destroy(spawnedEnemy);

            return;
        }

        aliveEnemyCount++;
        remainingEnemyCount++;

        trackedEnemies.Add(
            enemyStats
        );

        enemyStats.EnemyDied +=
            HandleEnemyDied;

        enemyStats.EnemyDeathCompleted +=
            HandleEnemyDeathCompleted;

        // 적의 비행선 타깃 연결
        // 이동 / 공격 타깃 모두 EnemyTargetSelector에서 관리
        EnemyTargetSelector targetSelector =
            spawnedEnemy.GetComponent<EnemyTargetSelector>();

        if (targetSelector != null)
        {
            targetSelector.SetAirshipTarget(
                target
            );
        }
        else
        {
            Debug.LogWarning(
                $"{spawnedEnemy.name}에 EnemyTargetSelector가 없습니다."
            );
        }
    }


    private void HandleEnemyDied(
        EnemyStats deadEnemy)
    {
        if (deadEnemy == null)
        {
            return;
        }

        deadEnemy.EnemyDied -=
            HandleEnemyDied;

        // HP 0이 된 순간 전투 가능한 적 수 감소
        aliveEnemyCount =
            Mathf.Max(
                aliveEnemyCount - 1,
                0
            );

        OnEnemyKilled?.Invoke();
    }

    private void HandleEnemyDeathCompleted(
        EnemyStats deadEnemy)
    {
        if (deadEnemy == null)
        {
            return;
        }

        deadEnemy.EnemyDeathCompleted -=
            HandleEnemyDeathCompleted;

        trackedEnemies.Remove(
            deadEnemy
        );

        // 사망 모션과 오브젝트 제거까지 끝난 적 수 감소
        remainingEnemyCount =
            Mathf.Max(
                remainingEnemyCount - 1,
                0
            );
    }

    private void RefreshAliveEnemyCount()
    {
        // 외부에서 직접 Destroy된 적 제거
        for (int i =
                 trackedEnemies.Count - 1;
             i >= 0;
             i--)
        {
            if (trackedEnemies[i] == null)
            {
                trackedEnemies.RemoveAt(i);
            }
        }

        aliveEnemyCount = 0;

        for (int i = 0;
             i < trackedEnemies.Count;
             i++)
        {
            EnemyStats enemy =
                trackedEnemies[i];

            if (enemy != null &&
                !enemy.IsDead)
            {
                aliveEnemyCount++;
            }
        }
    }

   

    // 실제 비행선의 HP가 0이 되었을 때 비행선 시스템에서 이 함수를 호출
    public void NotifyAirshipDestroyed()
    {
        if (isAirshipDestroyed)
        {
            return;
        }

        isAirshipDestroyed = true;

        Debug.Log(
            "비행선 HP가 0이 되었습니다."
        );
    }

  

    private void ResetRuntimeState()
    {
        currentWaveIndex = 0;
        aliveEnemyCount = 0;
        remainingEnemyCount = 0;
        remainingBossTime = 0f;

        isBossTimeOver = false;
        isAirshipDestroyed = false;
        isStageFinished = false;
    }

    private void ClearTrackedEnemies()
    {
        foreach (EnemyStats enemy
                 in trackedEnemies)
        {
            if (enemy == null)
            {
                continue;
            }

            enemy.EnemyDied -=
                HandleEnemyDied;

            enemy.EnemyDeathCompleted -=
                HandleEnemyDeathCompleted;

            Destroy(
                enemy.gameObject
            );
        }

        trackedEnemies.Clear();

        aliveEnemyCount = 0;
        remainingEnemyCount = 0;
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

        OnStageCompleted?.Invoke(
            currentStageNumber
        );
    }

    private void FailStage(
        string failureMessage)
    {
        if (isStageFinished)
        {
            return;
        }

        isStageFinished = true;
        stageRoutine = null;

        Debug.Log(
            failureMessage
        );

        OnStageFailed?.Invoke(
            currentStageNumber,
            failureMessage
        );
    }

    private void OnDisable()
    {
        StopStage();
    }
}
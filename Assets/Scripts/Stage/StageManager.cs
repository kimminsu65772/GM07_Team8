using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // 외부 시스템에서 스테이지 진행 상황을 받을 수 있는 이벤트
    public event Action OnEnemyKilled;
    public event Action<int> OnStageCompleted;
    public event Action<int> OnWaveCompleted;
    public event Action<int, int> OnStageStarted;
    public event Action<int, string> OnStageFailed;

    public int CurrentWave => currentWaveIndex + 1;
    public int TotalWaveCount { get; private set; }

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
    [SerializeField] private AirshipHealth airshipHealth;

    [Header("Boss UI")]
    [SerializeField] private BossTopHpUI bossTopHpUI;

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

        // 비활성화된 보스 HP UI까지 찾아서 연결
        if (bossTopHpUI == null)
        {
            bossTopHpUI =
                FindFirstObjectByType<BossTopHpUI>(
                    FindObjectsInactive.Include
                );
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

        // 런타임 생성 시 비활성화된 보스 HP UI까지 탐색
        if (bossTopHpUI == null)
        {
            bossTopHpUI =
                FindFirstObjectByType<BossTopHpUI>(
                    FindObjectsInactive.Include
                );
        }

        StopStage();

        currentStageNumber =
            stageData.StageNumber;

        TotalWaveCount =
            stageData.Waves.Count;

        SubscribeAirshipEvent();

        OnStageStarted?.Invoke(
            currentStageNumber,
            TotalWaveCount
        );

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

        UnsubscribeAirshipEvent();

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
        int spawnIndex,
        bool isBossWave)
    {
        // 위, 가운데, 아래 순서로 위치를 나눠서 생성
        float yOffset ;
        // 보스는 가운데에서 생성
        if (isBossWave)
        {
            yOffset = 0f;
        }
        else
        {
            // 일반 적은 아래 → 가운데 → 위 순서
            yOffset =
                (spawnIndex % 3 - 1) *
                spawnYOffset;
        }
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
        Debug.Log(
    $"보스 확인 - IsBossWave: {isBossWave}, IsBoss: {enemyStats.IsBoss}"
);
        // 실제 보스일 때만 상단 HP UI 연결
        // 보스 생성 시 UI가 없으면 다시 탐색
        if (isBossWave &&
            enemyStats.IsBoss)
        {
            if (bossTopHpUI == null)
            {
                bossTopHpUI =
                    FindFirstObjectByType<BossTopHpUI>(
                        FindObjectsInactive.Include
                    );
            }

            if (bossTopHpUI != null)
            {
                bossTopHpUI.SetBoss(
                    enemyStats
                );
            }
            else
            {
                Debug.LogWarning(
                    "StageManager: BossTopHpUI를 찾지 못했습니다."
                );
            }
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

        // 적한테 비행선 위치 전달
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

            OnWaveCompleted?.Invoke(
                currentWaveIndex + 1
            );

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
                if (isAirshipDestroyed)
                {
                    yield break;
                }

                SpawnEnemy(
                    spawnEntry.EnemyPrefab,
                    i,
                    waveData.IsBossWave
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

        if (isAirshipDestroyed)
        {
            yield break;
        }

        if (aliveEnemyCount > 0)
        {
            isBossTimeOver = true;
            yield break;
        }

        // 보스 사망 처리 완료까지 대기
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
            if (isAirshipDestroyed)
            {
                yield break;
            }

            elapsedTime +=
                Time.deltaTime;

            yield return null;
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

    private void SubscribeAirshipEvent()
    {
        if (airshipHealth == null)
        {
            return;
        }

        airshipHealth.OnDestroyed -=
            HandleAirshipDestroyed;

        airshipHealth.OnDestroyed +=
            HandleAirshipDestroyed;
    }

    private void UnsubscribeAirshipEvent()
    {
        if (airshipHealth == null)
        {
            return;
        }

        airshipHealth.OnDestroyed -=
            HandleAirshipDestroyed;
    }

    private void HandleAirshipDestroyed()
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
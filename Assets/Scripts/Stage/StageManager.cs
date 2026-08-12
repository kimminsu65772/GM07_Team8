using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Stage")]
    [SerializeField] private StageData stageData;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform target;

    [Header("Runtime Information")]
    // 플레이 중 현재 진행 상황을 Inspector에서 확인하기 위한 값
    [SerializeField] private int currentWaveIndex;
    [SerializeField] private int aliveEnemyCount;
    [SerializeField] private float remainingBossTime;
    [SerializeField] private bool isBossTimeOver;
    [SerializeField] private bool isAirshipDestroyed;
    [SerializeField] private bool isStageFinished;
    [SerializeField] private int remainingEnemyCount;

    // 생성한 적을 추적하여 사망 수와 이벤트 연결을 관리한다.
    private readonly List<EnemyStats> trackedEnemies =
        new List<EnemyStats>();

    private IEnumerator Start()
    {
        if (stageData == null ||
            spawnPoint == null ||
            target == null)
        {
            Debug.LogError(
                "StageManager의 StageData, SpawnPoint 또는 Target이 연결되지 않았습니다."
            );

            yield break;
        }
        
        // 스테이지 시작 상태 초기화
        currentWaveIndex = 0;
        aliveEnemyCount = 0;
        remainingBossTime = 0f;
        remainingEnemyCount = 0;
        isBossTimeOver = false;
        isAirshipDestroyed = false;
        isStageFinished = false;

       
        for (int i = 0; i < stageData.Waves.Count; i++)
        {
            currentWaveIndex = i;
            isBossTimeOver = false;

            WaveData currentWave =
                stageData.Waves[i];

            // 데이터에 등록된 적을 순서대로 생성한다.
            yield return StartCoroutine(
                SpawnWave(currentWave)
            );

            // 적을 생성하는 도중 비행선이 파괴되면 즉시 실패한다.
            if (isAirshipDestroyed || target == null)
            {
                FailStage("비행선 파괴: 스테이지 실패");
                yield break;
            }

            // 적 전멸, 비행선 파괴 또는 제한시간 초과까지 기다린다.
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

            // 마지막 웨이브 뒤에는 대기시간이 필요하지 않다.
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

                // 다음 웨이브 대기 중 비행선이 파괴된 경우
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
        // 웨이브에 등록된 적 구성을 순서대로 생성한다.
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
                // 비행선이 파괴됐다면 더 이상 적을 생성하지 않는다.
                if (target == null)
                {
                    isAirshipDestroyed = true;
                    yield break;
                }

                SpawnEnemy(
                    spawnEntry.EnemyPrefab
                );

                // 마지막 적을 생성한 뒤에는 기다리지 않는다.
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
        // 제한시간이 없는 일반 웨이브
        if (!waveData.IsBossWave ||
            waveData.TimeLimit <= 0f)
        {
            remainingBossTime = 0f;

            while (true)
            {
                RefreshAliveEnemyCount();

                if (remainingEnemyCount <= 0 ||
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

        // 보스 웨이브 제한시간 시작
        remainingBossTime =
            waveData.TimeLimit;

        while (true)
        {
            RefreshAliveEnemyCount();

            if (aliveEnemyCount <= 0 ||
                remainingBossTime <= 0f ||
                target == null)
            {
                break;
            }

            remainingBossTime -=
                Time.deltaTime;

            yield return null;
        }

        remainingBossTime = Mathf.Max(
            remainingBossTime,
            0f
        );

        // 비행선 파괴를 제한시간 초과보다 먼저 판정한다.
        if (target == null)
        {
            isAirshipDestroyed = true;
            yield break;
        }

        // 시간이 끝났는데 보스가 살아 있으면 실패한다.
        // 시간이 끝났는데 보스가 살아 있으면 실패한다.
        if (aliveEnemyCount > 0)
        {
            isBossTimeOver = true;
            yield break;
        }

        // 보스의 HP가 0이 된 뒤 사망 모션과 시체 제거가
        // 끝날 때까지 기다린 다음 웨이브를 완료한다.
        while (remainingEnemyCount > 0 &&
               target != null)
        {
            RefreshAliveEnemyCount();
            yield return null;
        }

        if (target == null)
        {
            isAirshipDestroyed = true;
        }
    }

    private IEnumerator WaitForDelay(
        float delay)
    {
        float elapsedTime = 0f;

        while (elapsedTime < delay)
        {
            // 웨이브 사이 또는 생성 간격 도중
            // 비행선이 파괴되면 대기를 중단한다.
            if (target == null)
            {
                isAirshipDestroyed = true;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void SpawnEnemy(
        GameObject enemyPrefab)
    {
        GameObject spawnedEnemy = Instantiate(
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

        trackedEnemies.Add(enemyStats);

        enemyStats.EnemyDied += HandleEnemyDied;
        enemyStats.EnemyDeathCompleted += HandleEnemyDeathCompleted;
        // 적 이동 스크립트에 비행선 타깃을 전달한다.
        EnemyMovement enemyMovement =
            spawnedEnemy.GetComponent<EnemyMovement>();

        if (enemyMovement != null)
        {
            enemyMovement.SetTarget(target);
        }

        // 적 공격 스크립트에도 같은 타깃을 전달한다.
        EnemyAttack enemyAttack =
            spawnedEnemy.GetComponent<EnemyAttack>();

        if (enemyAttack != null)
        {
            enemyAttack.SetTarget(target);
        }


    }

    private void HandleEnemyDied(EnemyStats deadEnemy)
    {
        // HP가 0이 된 적은 전투 가능한 적 수에서 제외한다.
        deadEnemy.EnemyDied -= HandleEnemyDied;

        aliveEnemyCount = Mathf.Max(
            aliveEnemyCount - 1,
            0);
    }
    private void HandleEnemyDeathCompleted(EnemyStats deadEnemy)
    {
        if (deadEnemy == null)
        {
            return;
        }

        // 사망 모션과 시체 제거가 끝난 시점의 이벤트 연결을 해제한다.
        deadEnemy.EnemyDeathCompleted -= HandleEnemyDeathCompleted;

        trackedEnemies.Remove(deadEnemy);

        remainingEnemyCount = Mathf.Max(
            remainingEnemyCount - 1,
            0);
    }
    private void RefreshAliveEnemyCount()
    {
        // 이벤트 없이 외부에서 삭제된 적도 생존 수에서 제외한다.
        for (int i = trackedEnemies.Count - 1;
             i >= 0;
             i--)
        {
            if (trackedEnemies[i] == null)
            {
                trackedEnemies.RemoveAt(i);
            }
        }

        aliveEnemyCount = 0;

        for (int i = 0; i < trackedEnemies.Count; i++)
        {
            EnemyStats enemy = trackedEnemies[i];

            if (enemy != null && !enemy.IsDead)
            {
                aliveEnemyCount++;
            }
        }
    }

    private void CompleteStage()
    {
        if (isStageFinished)
        {
            return;
        }

        isStageFinished = true;

        Debug.Log(
            "스테이지의 모든 웨이브가 완료되었습니다."
        );
    }

    private void FailStage(string failureMessage)
    {
        if (isStageFinished)
        {
            return;
        }

        isStageFinished = true;
        Debug.Log(failureMessage);
    }

    private void OnDisable()
    {
        // StageManager가 비활성화되거나 씬이 종료될 때
        // 남아 있는 적의 이벤트 연결을 모두 해제한다.
        foreach (EnemyStats enemy
                 in trackedEnemies)
        {
            if (enemy != null)
            {
                enemy.EnemyDied -=
                    HandleEnemyDied;
            }
        }

        trackedEnemies.Clear();
    }
}
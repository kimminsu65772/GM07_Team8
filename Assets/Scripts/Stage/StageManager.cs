using System.Collections;
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

    private IEnumerator Start()
    {
        if (stageData == null ||
            spawnPoint == null ||
            target == null)
        {
            Debug.LogError(
                "StageManager의 StageData, SpawnPoint 또는 Target이 연결되지 않았습니다.");

            yield break;
        }

        // 스테이지 시작 상태 초기화
        aliveEnemyCount = 0;
        remainingBossTime = 0f;
        isBossTimeOver = false;
        isAirshipDestroyed = false;

        for (int i = 0; i < stageData.Waves.Count; i++)
        {
            currentWaveIndex = i;

            WaveData currentWave = stageData.Waves[i];

            yield return StartCoroutine(
                SpawnWave(currentWave));

            isBossTimeOver = false;

            // 적 전멸, 비행선 파괴, 보스 시간 초과 중하나가 발생할 때까지 기다린다.
            yield return StartCoroutine(
                WaitForWaveClear(currentWave));

            if (isAirshipDestroyed)
            {
                Debug.Log("비행선 파괴: 스테이지 실패");
                yield break;
            }

            if (isBossTimeOver)
            {
                Debug.Log("보스 제한시간 초과: 스테이지 실패");
                yield break;
            }

            if (currentWave.NextWaveDelay > 0f)
            {
                yield return new WaitForSeconds(
                    currentWave.NextWaveDelay);
            }
        }

        Debug.Log("스테이지의 모든 웨이브가 완료되었습니다.");
    }
    private IEnumerator WaitForWaveClear(WaveData waveData)
    {
        // 일반 웨이브는 적 전멸 또는 비행선 파괴까지 기다린다.
        if (!waveData.IsBossWave ||
            waveData.TimeLimit <= 0f)
        {
            remainingBossTime = 0f;

            while (aliveEnemyCount > 0 &&
                   target != null)
            {
                yield return null;
            }

            if (target == null)
            {
                isAirshipDestroyed = true;
            }

            yield break;
        }

        // 보스 웨이브 제한시간 시작
        remainingBossTime = waveData.TimeLimit;

        while (aliveEnemyCount > 0 &&
               remainingBossTime > 0f &&
               target != null)
        {
            remainingBossTime -= Time.deltaTime;
            yield return null;
        }

        remainingBossTime = Mathf.Max(
            remainingBossTime,
            0f);

        if (target == null)
        {
            isAirshipDestroyed = true;
            yield break;
        }

        // 시간이 끝났는데 보스가 남아 있으면 실패
        if (aliveEnemyCount > 0)
        {
            isBossTimeOver = true;
        }
    }
    private IEnumerator SpawnWave(WaveData waveData)
    {
        // 웨이브에 등록된 적 구성들을 순서대로 생성한다.
        foreach (EnemySpawnEntry spawnEntry in waveData.EnemySpawns)
        {
            if (spawnEntry.EnemyPrefab == null)
            {
                Debug.LogWarning(
                    $"{waveData.name}에 Enemy Prefab이 없습니다.");

                continue;
            }

            for (int i = 0; i < spawnEntry.SpawnCount; i++)
            {
                SpawnEnemy(spawnEntry.EnemyPrefab);

                // 마지막 적을 생성한 뒤에는 기다릴 필요가 없다.
                if (i < spawnEntry.SpawnCount - 1 &&
                    spawnEntry.SpawnInterval > 0f)
                {
                    yield return new WaitForSeconds(
                        spawnEntry.SpawnInterval);
                }
            }
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        GameObject spawnedEnemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            Quaternion.identity);

        // 생성된 적에게 현재 비행선 타깃을 전달한다.
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

        EnemyStats enemyStats =
            spawnedEnemy.GetComponent<EnemyStats>();

        if (enemyStats == null)
        {
            Debug.LogError(
                $"{spawnedEnemy.name}에 EnemyStats가 없습니다.");

            Destroy(spawnedEnemy);
            return;
        }

        // 살아 있는 적의 수를 올리고 사망 이벤트를 구독한다.
        aliveEnemyCount++;
        enemyStats.EnemyDied += HandleEnemyDied;
    }

    private void HandleEnemyDied(EnemyStats deadEnemy)
    {
        // 파괴되기 전에 이벤트 연결을 해제한다.
        deadEnemy.EnemyDied -= HandleEnemyDied;

        // 적이 사망할 때마다 생존 적 수를 감소시킨다.
        aliveEnemyCount = Mathf.Max(
            aliveEnemyCount - 1,
            0);
    }
}
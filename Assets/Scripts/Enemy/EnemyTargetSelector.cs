using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]

public class EnemyTargetSelector : MonoBehaviour
{
    [Header("Fallback Target")]
    [SerializeField] private Transform airshipTarget;

    private EnemyMovement enemyMovement;
    private EnemyAttack enemyAttack;
    private Transform currentTarget;
    private EnemyRangedAttack enemyRangedAttack;
    private EnemyMagicAttack enemyMagicAttack;
    private BattleManager battleManager;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyRangedAttack = GetComponent<EnemyRangedAttack>();
        enemyMagicAttack = GetComponent<EnemyMagicAttack>();
        battleManager = BattleManager.Instance;
    }

    private void Start()
    {
        RefreshTarget();
    }

    private void Update()
    {
        Transform priorityTarget =
             GetPriorityTarget(transform.position);

        if (priorityTarget == currentTarget)
        {
            return;
        }

        currentTarget = priorityTarget;

        // 영웅 또는 비행선으로 공격 대상만 변경
        enemyAttack?.SetTarget(currentTarget);
        enemyRangedAttack?.SetTarget(currentTarget);
        enemyMovement?.SetCombatTarget(currentTarget);
        enemyMagicAttack?.SetTarget(currentTarget);
    }

    public void SetAirshipTarget(Transform newAirshipTarget)
    {
        airshipTarget = newAirshipTarget;

        // 이동은 항상 비행선 방향
        enemyMovement?.SetTarget(airshipTarget);

        // 공격 타깃은 영웅 우선으로 다시 탐색
        RefreshTarget();
    }

    public void RefreshTarget()
    {
        currentTarget =
            GetPriorityTarget(transform.position);

        enemyAttack?.SetTarget(currentTarget);
        enemyRangedAttack?.SetTarget(currentTarget);
        enemyMovement?.SetCombatTarget(currentTarget);
        enemyMagicAttack?.SetTarget(currentTarget);
    }

    public Transform GetPriorityTarget(Vector3 enemyPosition)
    {
        Transform closestHero = null;
        float closestDistanceSqr = float.MaxValue;

        if (battleManager == null)
        {
            return airshipTarget;
        }

        IReadOnlyList<Hero> spawnedHeroes =
            battleManager.SpawnedHeroes;

        for (int i = 0; i < spawnedHeroes.Count; i++)
        {
            Hero hero = spawnedHeroes[i];

            if (hero == null ||
                !hero.gameObject.activeInHierarchy)
            {
                continue;
            }

            // 죽어 있는 영웅은 제외
            if (hero.IsDead ||
                hero.HeroCurrentHP <= 0f)
            {
                continue;
            }

            Vector2 offset =
                hero.transform.position - enemyPosition;

            float distanceSqr =
                offset.sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestHero = hero.transform;
            }
        }

        // 살아있는 영웅이 하나라도 있으면 무조건 영웅 우선
        if (closestHero != null)
        {
            return closestHero;
        }

        // 살아있는 영웅이 없을 때만 비행선
        return airshipTarget;
    }
}

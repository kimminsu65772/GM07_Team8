using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyAttack))]
public class BossEnragePattern : MonoBehaviour
{
    [Header("Enrage")]
    [SerializeField, Range(0f, 1f)]
    private float healthThreshold = 0.3f;

    [SerializeField, Min(0.1f)]
    private float attackSpeedMultiplier = 2f;

    [SerializeField, Min(1f)]
    private float sizeMultiplier = 1.2f;

    private EnemyStats enemyStats;
    private EnemyAttack enemyAttack;

    private bool isEnraged;
    private Vector3 originalScale;

    private void Awake()
    {
        enemyStats =
            GetComponent<EnemyStats>();

        enemyAttack =
            GetComponent<EnemyAttack>();

        // 기존 크기 저장
        originalScale =
            transform.localScale;
    }

    private void OnEnable()
    {
        if (enemyStats == null)
        {
            return;
        }

        enemyStats.EnemyDamaged +=
            HandleEnemyDamaged;
    }

    private void HandleEnemyDamaged(
        EnemyStats damagedEnemy)
    {
        if (isEnraged ||
            damagedEnemy.IsDead)
        {
            return;
        }

        float healthRatio =
            (float)damagedEnemy.CurrentHealth /
            damagedEnemy.MaxHealth;

        // 체력 30% 이하에서 특수패턴 시작
        if (healthRatio <= healthThreshold)
        {
            ActivateEnrage();
        }
    }

    private void ActivateEnrage()
    {
        isEnraged = true;

        // 공격속도 증가
        enemyAttack.SetAttackSpeedMultiplier(
            attackSpeedMultiplier
        );

        // 보스 크기 증가
        transform.localScale =
            originalScale *
            sizeMultiplier;

        Debug.Log(
            "보스 광폭화 - 공격속도 및 크기 증가"
        );
    }

    private void OnDisable()
    {
        if (enemyStats == null)
        {
            return;
        }

        enemyStats.EnemyDamaged -=
            HandleEnemyDamaged;
    }
}
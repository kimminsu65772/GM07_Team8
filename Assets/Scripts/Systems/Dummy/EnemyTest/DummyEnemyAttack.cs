using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class DummyEnemyAttack : MonoBehaviour
{
    [SerializeField] private Transform target;

    private EnemyStats enemyStats;
    private IDamageable targetDamageable;
    private float attackTimer;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
        ResetAttackTimer();
    }

    private void Update()
    {
        if (target == null ||
            targetDamageable == null ||
            enemyStats == null ||
            enemyStats.IsDead)
        {
            ResetAttackTimer();
            return;
        }

        float horizontalDistanceToTarget =
            Mathf.Abs(transform.position.x - target.position.x);

        if (horizontalDistanceToTarget > enemyStats.AttackRange)
        {
            ResetAttackTimer();
            return;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer > 0f)
        {
            return;
        }

        targetDamageable.TakeDamage(enemyStats.AttackPower);
        ResetAttackTimer();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetDamageable = target != null
            ? target.GetComponentInParent<IDamageable>()
            : null;

        if (target != null && targetDamageable == null)
        {
            Debug.LogWarning($"{name}: target에서 IDamageable을 찾지 못했습니다.");
        }

        ResetAttackTimer();
    }

    private void ResetAttackTimer()
    {
        attackTimer = enemyStats != null
            ? enemyStats.AttackInterval
            : 0f;
    }
}

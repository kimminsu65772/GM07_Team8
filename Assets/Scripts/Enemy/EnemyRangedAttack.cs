using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyStats))]
public class EnemyRangedAttack : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Attack Event")]
    [SerializeField] private UnityEvent attackPerformed = new UnityEvent();

    private EnemyStats enemyStats;
    private float attackTimer;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
        ResetAttackTimer();
    }

    private void Update()
    {
        if (target == null ||
            enemyStats == null ||
            enemyStats.IsDead)
        {
            ResetAttackTimer();
            return;
        }

        float distance =
            Mathf.Abs(
                transform.position.x -
                target.position.x
            );

        if (distance > enemyStats.AttackRange)
        {
            ResetAttackTimer();
            return;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer > 0f)
        {
            return;
        }

        Attack();
        ResetAttackTimer();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        ResetAttackTimer();
    }

    private void Attack()
    {
        if (target == null)
        {
            return;
        }

        attackPerformed?.Invoke();
    }

    // 공격 애니메이션 이벤트에서 호출
    public void FireProjectile()
    {
        if (projectilePrefab == null ||
            firePoint == null ||
            target == null)
        {
            return;
        }

        GameObject projectile =
            Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity
            );

        EnemyProjectile enemyProjectile =
            projectile.GetComponent<EnemyProjectile>();

        if (enemyProjectile != null)
        {
            enemyProjectile.Init(
                target,
                enemyStats.AttackPower
            );
        }
    }

    private void ResetAttackTimer()
    {
        if (enemyStats == null)
        {
            attackTimer = 0f;
            return;
        }

        attackTimer =
            enemyStats.AttackInterval;
    }
}
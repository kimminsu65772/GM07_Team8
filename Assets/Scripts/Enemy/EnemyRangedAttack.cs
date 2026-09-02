using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyStats))]
public class EnemyRangedAttack : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Projectile")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileScale = 1f;

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
        if (target == null || enemyStats == null || enemyStats.IsDead || enemyStats.IsStunned)
        {
            ResetAttackTimer();
            return;
        }

        float distance =
            Mathf.Abs( transform.position.x - target.position.x );

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
        if (target == null || enemyStats == null ||  enemyStats.IsDead || enemyStats.IsStunned)
        {
            return;
        }

        attackPerformed?.Invoke();
    }

    // 공격 애니메이션 이벤트에서 호출
    public void FireProjectile()
    {
        if (firePoint == null || target == null || enemyStats == null || enemyStats.IsDead || enemyStats.IsStunned ||  PoolingManager.Instance == null)
        {
            return;
        }

        EnemyProjectile projectile = PoolingManager.Instance.GetEnemyProjectile();

        if (projectile == null)
        {
            return;
        }

        projectile.transform.localScale =  Vector3.one * projectileScale;

        projectile.Init( firePoint.position, Quaternion.identity, target, enemyStats.AttackPower );
    }

    private void ResetAttackTimer()
    {
        if (enemyStats == null)
        {
            attackTimer = 0f;
            return;
        }

        attackTimer =  enemyStats.AttackInterval;
    }
    public void FireProjectile(Transform spawnPoint)
    {
        if (target == null || spawnPoint == null || enemyStats == null || enemyStats.IsDead || enemyStats.IsStunned || PoolingManager.Instance == null)
        {
            return;
        }


        // 풀에서 받아온 투사체를 지정한 발사 위치에서 생성한다.
        EnemyProjectile projectile = PoolingManager.Instance.GetEnemyProjectile();
        if (projectile == null)
        {
            return;
        }

        projectile.transform.localScale = Vector3.one * projectileScale;
        projectile.Init(spawnPoint.position, spawnPoint.rotation, target, enemyStats.AttackPower);
    }
}

using UnityEngine;

public class AirshipAttack : MonoBehaviour
{
    [SerializeField] private AirshipEnemyChecker enemyChecker;
    [SerializeField] private AirshipHealth health;
    [SerializeField] private AirshipProjectileBase projectilePrefab;
    [SerializeField] private Transform attackPoint;

    private float attackDamage;
    private float attackInterval = 1f;
    private float attackTimer;
    
    private Transform cachedTarget;
    private IDamageable cachedDamageable;

    private void Awake()
    {
        if (enemyChecker == null)
            enemyChecker = GetComponent<AirshipEnemyChecker>();

        if (health == null)
            health = GetComponent<AirshipHealth>();
    }

    // 쿨타임 및 적 감지
    private void Update()
    {
        if (health != null && health.IsDestroyed)
            return;
        
        attackTimer -= Time.deltaTime;

        if (attackTimer > 0f)
            return;

        Transform target = enemyChecker.FindNearestEnemy();

        if (target == null)
            return;

        Attack(target);
        attackTimer = attackInterval;
    }

    public void ApplyStats(AirshipRuntimeStats stats)
    {
        if (stats == null)
            return;

        attackDamage = stats.Attack;
        attackInterval = stats.AttackSpeed <= 0f ? 1f : 1f / stats.AttackSpeed;
    }

    // 캐싱해 둔 타겟이랑 다르면 겟컴포넌트
    private void Attack(Transform target)
    {
        Vector2 direction =
            (Vector2)target.position - (Vector2)attackPoint.position;

        if (direction.sqrMagnitude > 0f)
        {
            attackPoint.right = direction.normalized;
        }
        
        if (target != cachedTarget)
        {
            cachedTarget = target;
            cachedDamageable =
                target.GetComponentInParent<IDamageable>();
        }

        if (cachedDamageable == null)
            return;

        AirshipProjectileBase projectile = Instantiate(
            projectilePrefab,
            attackPoint.position,
            attackPoint.rotation
        );

        projectile.Init(
            target,
            cachedDamageable,
            attackDamage
        );
    }
}
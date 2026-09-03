using UnityEngine;

public enum HeroProjectileType
{
    None,
    PlayerAttackProjectile1,
    PlayerAttackProjectile2,
    PlayerAttackProjectile3,
    PlayerAttackArrow,
    PlayerSkillArrow
}

public class HeroAttackProjectileController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField, Min(0.1f)] private float lifeTime = 3f;

    protected Transform targetPos;
    private EnemyStats target;
    private DamageInfo damageInfo;
    private float remainingLifeTime;

    private PoolingManager poolingManager;
    private HeroProjectileType poolingType;

    public void SetPoolingManager(
        PoolingManager poolingManager,
        HeroProjectileType poolingType)
    {
        this.poolingManager = poolingManager;
        this.poolingType = poolingType;
    }

    public void Init(
        Vector3 startPosition,
        Quaternion startRotation,
        Transform target,
        DamageInfo damageInfo)
    {
        transform.SetPositionAndRotation(
            startPosition,
            startRotation
        );

        targetPos = target;
        this.damageInfo = damageInfo;
        this.target = targetPos.GetComponent<EnemyStats>();
        remainingLifeTime = lifeTime;

        gameObject.SetActive(true);
    }

    protected virtual void Update()
    {
        if (target == null || target.IsDead)
        {
            ReturnToPool();
            return;
        }

        remainingLifeTime -= Time.deltaTime;

        if (remainingLifeTime <= 0f)
        {
            ReturnToPool();
            return;
        }

        Vector2 direction = targetPos.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPos.position,
            moveSpeed * Time.deltaTime
        );

        if (target.TryGetComponent<IDamageable>(
                out IDamageable enemy))
        {
            float distance = Vector2.Distance(
                transform.position,
                targetPos.position
            );

            if (distance <= enemy.HitRadius)
            {
                EnemyStats enemyStats = target.GetComponent<EnemyStats>();
                if (!target.gameObject.activeSelf || enemyStats.IsDead) return;

                enemy.TakeDamage(damageInfo);
                ReturnToPool();
            }
        }
    }

    protected void ReturnToPool()
    {
        if (poolingManager == null)
        {
            Debug.LogError(
                "영웅 투사체에 PoolingManager가 연결되지 않았습니다.",
                this
            );

            gameObject.SetActive(false);
            return;
        }

        poolingManager.ReleaseHeroProjectile(
            this,
            poolingType
        );
    }
}
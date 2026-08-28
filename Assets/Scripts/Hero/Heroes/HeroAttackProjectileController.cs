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

    private Transform target;
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

        this.target = target;
        this.damageInfo = damageInfo;
        remainingLifeTime = lifeTime;

        gameObject.SetActive(true);
    }

    protected virtual void Update()
    {
        if (target == null)
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

        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        if (target.TryGetComponent<IDamageable>(
                out IDamageable enemy))
        {
            float distance = Vector2.Distance(
                transform.position,
                target.position
            );

            if (distance <= enemy.HitRadius)
            {
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
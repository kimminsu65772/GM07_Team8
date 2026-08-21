using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField, Min(0.1f)]
    private float moveSpeed = 8f;

    [SerializeField, Min(0.1f)]
    private float lifeTime = 5f;

    [SerializeField, Min(0.01f)]
    private float hitDistance = 0.3f;

    private Transform target;
    private IDamageable targetDamageable;
    private float damage;
    private float remainingLifeTime;

    private PoolingManager poolingManager;

    public void SetPoolingManager(
        PoolingManager poolingManager)
    {
        this.poolingManager = poolingManager;
    }

    public void Init(
        Vector3 startPosition,
        Quaternion startRotation,
        Transform newTarget,
        float newDamage)
    {
        transform.SetPositionAndRotation(
            startPosition,
            startRotation
        );

        target = newTarget;

        targetDamageable =
            target != null
                ? target.GetComponentInParent<IDamageable>()
                : null;

        damage = newDamage;
        remainingLifeTime = lifeTime;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (target == null ||
            targetDamageable == null)
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

        Vector3 direction =
            (target.position - transform.position).normalized;

        // 기존 회전 방식 유지
        transform.up = direction;

        transform.position +=
            direction *
            moveSpeed *
            Time.deltaTime;

        float distance =
            Vector2.Distance(
                transform.position,
                target.position
            );

        if (distance <= hitDistance)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (targetDamageable != null)
        {
            targetDamageable.TakeDamage(
                new DamageInfo(damage)
            );
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (poolingManager == null)
        {
            Debug.LogError(
                "적 투사체에 PoolingManager가 연결되지 않았습니다.",
                this
            );

            gameObject.SetActive(false);
            return;
        }

        poolingManager.ReleaseEnemyProjectile(this);
    }
}
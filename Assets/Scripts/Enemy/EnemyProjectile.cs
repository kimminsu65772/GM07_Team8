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

    public void Init(
        Transform newTarget,
        float newDamage)
    {
        target = newTarget;

        targetDamageable =
            target != null
            ? target.GetComponentInParent<IDamageable>()
            : null;

        damage = newDamage;

        Destroy(
            gameObject,
            lifeTime
        );
    }

    private void Update()
    {
        if (target == null ||
            targetDamageable == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction =
            (target.position - transform.position).normalized;

        // 화살이 이동 방향을 바라보도록 회전
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
        if (targetDamageable == null)
        {
            return;
        }

        targetDamageable.TakeDamage(
            new DamageInfo(damage)
        );

        Destroy(gameObject);
    }
}
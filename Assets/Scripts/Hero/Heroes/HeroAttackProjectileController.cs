using UnityEngine;

public class HeroAttackProjectileController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Transform target;
    private DamageInfo damageInfo;

    public void Init(Transform target, DamageInfo damageInfo)
    {
        this.target = target;
        this.damageInfo = damageInfo;
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

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
                Destroy(gameObject);
            }
        }
    }
}
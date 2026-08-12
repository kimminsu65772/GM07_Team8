using UnityEngine;

public class HeroAttackProjectileController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Transform target;
    private float damage;

    public void Init(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;
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
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
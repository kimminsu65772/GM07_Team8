using UnityEngine;

public class DummyEnemyDamageable : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private float hitRadius = 0.5f;
    [SerializeField] private bool dieInOneHit = true;

    public float HitRadius => hitRadius;

    private void Awake()
    {
        if (enemyStats == null)
        {
            enemyStats = GetComponent<EnemyStats>();
        }
    }

    public void TakeDamage(float damage)
    {
        if (enemyStats == null || damage <= 0f || enemyStats.IsDead)
        {
            return;
        }

        int damageAmount = Mathf.Max(1, Mathf.CeilToInt(damage));

        if (dieInOneHit)
        {
            damageAmount = Mathf.Max(damageAmount, enemyStats.CurrentHealth);
        }

        enemyStats.TakeDamage(damageAmount);
    }
}

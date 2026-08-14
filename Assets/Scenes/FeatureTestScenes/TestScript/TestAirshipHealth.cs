using UnityEngine;

public class TestAirshipHealth : MonoBehaviour, IDamageable
{
    [Header("Test Health")]
    [SerializeField, Min(1)] private int maxHealth = 30;
    [SerializeField] private int currentHealth;
    [Header("Hit")]
    [SerializeField] private float hitRadius = 1f;

    public float HitRadius => hitRadius;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        float damageAmount = damageInfo.Damage;
        if (damageAmount <= 0f || currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(
            currentHealth - Mathf.RoundToInt(damageAmount),
            0);

        Debug.Log(
            $"비행선 피격! 피해량: {damageAmount}, 남은 체력: {currentHealth}");

        if (currentHealth == 0)
        {
            Destroy(gameObject);
        }
    }
}
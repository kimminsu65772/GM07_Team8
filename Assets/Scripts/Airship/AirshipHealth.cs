using System;
using UnityEngine;

/// <summary>
/// 비행선의 체력 관련.
/// </summary>
public class AirshipHealth : MonoBehaviour
{
    private float maxHealth;
    [SerializeField]private float currentHealth;
    private float shield;
    private bool isDestroyed;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float Shield => shield;
    public bool IsDestroyed => isDestroyed;

    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnDamaged;
    public event Action<float> OnHealed;
    public event Action<float> OnShieldChanged;
    public event Action OnDestroyed;

    public void ApplyStats(AirshipRuntimeStats stats)
    {
        if (stats == null)
        {
            return;
        }

        float previousMaxHealth = maxHealth;
        maxHealth = stats.MaxHealth;

        // 첫 적용이면 풀피. 안쓸수도 있음.
        if (previousMaxHealth <= 0f)
        {
            currentHealth = maxHealth;
        }
        // 체력 증가분 만큼 현재 체력 증가.
        else
        {
            float increasedHealth = maxHealth - previousMaxHealth;
            currentHealth = Mathf.Clamp(currentHealth + increasedHealth, 0f, maxHealth);
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // 리스폰 및 스테이지 입장 등에 씀.
    public void ResetHealth()
    {
        isDestroyed = false;
        shield = 0f;
        currentHealth = maxHealth;

        OnShieldChanged?.Invoke(shield);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed || damage <= 0f)
        {
            return;
        }

        float remainingDamage = damage;

        // 쉴드가 있으면 먼저 차감.
        if (shield > 0f)
        {
            float absorbedDamage = Mathf.Min(shield, remainingDamage);
            shield -= absorbedDamage;
            remainingDamage -= absorbedDamage;

            OnShieldChanged?.Invoke(shield);
        }

        if (remainingDamage <= 0f)
        {
            OnDamaged?.Invoke(damage);
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - remainingDamage);

        OnDamaged?.Invoke(damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            DestroyAirship();
        }
    }

    public void Heal(float amount)
    {
        if (isDestroyed || amount <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        OnHealed?.Invoke(amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void AddShield(float amount)
    {
        if (isDestroyed || amount <= 0f)
        {
            return;
        }

        shield += amount;
        OnShieldChanged?.Invoke(shield);
    }

    private void DestroyAirship()
    {
        if (isDestroyed)
        {
            return;
        }

        isDestroyed = true;
        currentHealth = 0f;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDestroyed?.Invoke();
    }
}
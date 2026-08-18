using System;
using UnityEngine;

/// <summary>
/// 비행선의 체력 관련.
/// </summary>
public class AirshipHealth : MonoBehaviour, IDamageable
{
    [SerializeField]
    private AirshipEquipmentController equipmentController;
    
    private float maxHealth;
    [SerializeField]private float currentHealth;
    [SerializeField]private float shield;
    private bool isDestroyed;
    [SerializeField] private float hitRadius = 1f;
    [SerializeField] private bool drawOnlyWhenSelected = false;
    
    private bool shieldEnabled;
    private float shieldRegenTimer;
    private bool isShieldRegenWaiting;
    private float shieldRegenDelay = 6f;

    public float HitRadius => hitRadius;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float Shield => shield;
    public bool IsDestroyed => isDestroyed;
    public bool IsShieldEnabled => shieldEnabled;

    public event Action<float, float> OnHealthChanged;
    public event Action<DamageInfo> OnDamaged;
    public event Action<float> OnHealed;
    public event Action<float> OnShieldChanged;
    public event Action OnDestroyed;
    
    private void Awake()
    {
        if (equipmentController == null)
            equipmentController =
                GetComponent<AirshipEquipmentController>();
    }
    private void OnEnable()
    {
        equipmentController.OnGearChanged += HandleGearChanged;
        HandleGearChanged(equipmentController.EquippedGear);
    }

    private void OnDisable()
    {
        equipmentController.OnGearChanged -= HandleGearChanged;
    }
    private void Update()
    {
        if (isDestroyed || !isShieldRegenWaiting)
            return;

        shieldRegenTimer -= Time.deltaTime;

        if (shieldRegenTimer > 0f)
            return;

        isShieldRegenWaiting = false;

        if (!shieldEnabled)
            return;

        shield = maxHealth;
        OnShieldChanged?.Invoke(shield);
    }
    private void HandleGearChanged(AirshipGearData gear)
    {
        bool hasShield =
            gear != null &&
            gear.GearType == AirshipGearType.Shield;

        SetShieldEnabled(hasShield);
    }
    public void SetShieldEnabled(bool enabled)
    {
        bool wasShieldEnabled = shieldEnabled;
        shieldEnabled = enabled;

        if (!enabled)
        {
            // 실드가 일부라도 깎인 상태에서 해제하면 쿨타임 시작
            if (wasShieldEnabled &&
                shield < maxHealth &&
                !isShieldRegenWaiting)
            {
                shieldRegenTimer = shieldRegenDelay;
                isShieldRegenWaiting = true;
            }

            shield = 0f;
            OnShieldChanged?.Invoke(shield);
            return;
        }

        // 같은 실드 부품을 반복 장착해도 현재 상태 유지
        if (wasShieldEnabled)
        {
            OnShieldChanged?.Invoke(shield);
            return;
        }

        // 쿨타임 중 재장착이면 충전하지 않음
        shield = isShieldRegenWaiting
            ? 0f
            : maxHealth;

        OnShieldChanged?.Invoke(shield);
    }

    public void ApplyStats(AirshipRuntimeStats stats)
    {
        if (stats == null)
            return;

        float previousMaxHealth = maxHealth;

        float previousHealthRatio =
            previousMaxHealth > 0f
                ? Mathf.Clamp01(currentHealth / previousMaxHealth)
                : 1f;

        maxHealth = stats.MaxHealth;

        // 최초 스탯 적용
        if (previousMaxHealth <= 0f)
        {
            currentHealth = maxHealth;
        }
        // 최대 체력 감소시 현재 체력 비율 유지
        else if (maxHealth < previousMaxHealth)
        {
            currentHealth = maxHealth * previousHealthRatio;
        }
        else
        {
            float increasedHealth =
                maxHealth - previousMaxHealth;

            currentHealth = Mathf.Clamp(
                currentHealth + increasedHealth,
                0f,
                maxHealth
            );
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // 리스폰 및 스테이지 입장 등에 씀.
    public void ResetHealth()
    {
        isDestroyed = false;
        shieldRegenTimer = 0f;
        isShieldRegenWaiting = false;
        shield = shieldEnabled ? maxHealth : 0f;
        currentHealth = maxHealth;

        OnShieldChanged?.Invoke(shield);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (isDestroyed || damageInfo.Damage <= 0f)
            return;

        float remainingDamage = damageInfo.Damage;

        float previousShield = shield;

        if (shieldEnabled && shield > 0f)
        {
            float absorbedDamage =
                Mathf.Min(shield, remainingDamage);

            shield -= absorbedDamage;
            remainingDamage -= absorbedDamage;

            OnShieldChanged?.Invoke(shield);

            if (previousShield > 0f && shield <= 0f)
            {
                shieldRegenTimer = shieldRegenDelay;
                isShieldRegenWaiting = true;
            }
        }

        // 보호막이 전부 막았으면 실제 체력 피해 없음
        if (remainingDamage <= 0f)
            return;

        // 실제로 깎이는 체력만 계산
        float appliedDamage =
            Mathf.Min(currentHealth, remainingDamage);

        currentHealth -= appliedDamage;

        OnDamaged?.Invoke(
            new DamageInfo(
                appliedDamage,
                damageInfo.IsCritical
            )
        );

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        if (currentHealth <= 0f)
            DestroyAirship();
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

    // public void AddShield(float amount)
    // {
    //     if (!shieldEnabled ||
    //         isDestroyed ||
    //         amount <= 0f)
    //     {
    //         return;
    //     }
    //
    //     shield += amount;
    //     OnShieldChanged?.Invoke(shield);
    // }

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
    private void OnDrawGizmos()
    {
        if (drawOnlyWhenSelected)
            return;

        DrawGizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawOnlyWhenSelected)
            return;

        DrawGizmo();
    }

    private void DrawGizmo()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
    
    [ContextMenu("TestResetHp")]
    private void TestResetHp()
    {
        maxHealth = 100f;
        ResetHealth();
    }
    [ContextMenu("Test Damage")]
    private void TestDamage()
    {
        TakeDamage(new DamageInfo(10f));
    }
    [ContextMenu("Test Heal")]
    private void TestHeal()
    {
        Heal(10f);
    }
}
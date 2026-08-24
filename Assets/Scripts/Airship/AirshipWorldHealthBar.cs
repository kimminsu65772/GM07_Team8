using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AirshipWorldHealthBar : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private AirshipHealth health;
    [SerializeField] private Slider currentHealthSlider;
    [SerializeField] private Slider delayedHealthSlider;
    [SerializeField] private Slider shieldHealthSlider;
    [SerializeField] private TMP_Text healthText;

    [Header("연출")]
    [SerializeField] private float currentSmoothTime = 0.1f;
    [SerializeField] private float shieldSmoothTime = 0.1f;
    [SerializeField] private float delayedHealthDelay = 0.4f;
    [SerializeField] private float delayedSmoothTime = 0.2f;

    private float currentTarget;
    private float delayedTarget;
    private float shieldTarget;

    private float currentVelocity;
    private float delayedVelocity;
    private float shieldVelocity;

    private float delayedTimer;

    private bool delayedFollowing;
    private bool initialized;

    // 최대 체력 변경 감지용
    private double lastMaxHealth = -1d;

    private void OnEnable()
    {
        health.OnHealthChanged += HandleHealthChanged;
        health.OnShieldChanged += HandleShieldChanged;

        HandleHealthChanged(
            health.CurrentHealth,
            health.MaxHealth
        );

        HandleShieldChanged(health.Shield);
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= HandleHealthChanged;
        health.OnShieldChanged -= HandleShieldChanged;
    }

    private void Update()
    {
        currentHealthSlider.normalizedValue =
            Mathf.SmoothDamp(
                currentHealthSlider.normalizedValue,
                currentTarget,
                ref currentVelocity,
                currentSmoothTime
            );

        shieldHealthSlider.normalizedValue =
            Mathf.SmoothDamp(
                shieldHealthSlider.normalizedValue,
                shieldTarget,
                ref shieldVelocity,
                shieldSmoothTime
            );

        if (!delayedFollowing)
            return;

        if (delayedTimer > 0f)
        {
            delayedTimer -= Time.deltaTime;
            return;
        }

        delayedHealthSlider.normalizedValue =
            Mathf.SmoothDamp(
                delayedHealthSlider.normalizedValue,
                delayedTarget,
                ref delayedVelocity,
                delayedSmoothTime
            );

        if (Mathf.Abs(
                delayedHealthSlider.normalizedValue -
                delayedTarget) <= 0.001f)
        {
            delayedHealthSlider.normalizedValue =
                delayedTarget;

            delayedVelocity = 0f;
            delayedFollowing = false;
        }
    }

    private void HandleHealthChanged(
        double currentHealth,
        double maxHealth)
    {
        UpdateHealthText(
            currentHealth,
            maxHealth,
            health.Shield
        );

        double currentShield = health.Shield;

        double barMax =
            GetBarMax(
                currentHealth,
                maxHealth,
                currentShield
            );

        float targetRatio =
            barMax <= 0d
                ? 0f
                : (float)Math.Max(
                    0d,
                    Math.Min(1d, currentHealth / barMax)
                );

        UpdateShieldSlider(
            currentHealth,
            currentShield,
            barMax
        );

        // 최초 초기화 또는 최대 체력 자체가 변경된 경우만 즉시 초기화
        if (!initialized ||
            lastMaxHealth != maxHealth)
        {
            SnapTo(
                targetRatio,
                shieldTarget
            );

            initialized = true;
            lastMaxHealth = maxHealth;
            return;
        }

        lastMaxHealth = maxHealth;
        currentTarget = targetRatio;

        // 회복
        if (targetRatio >= delayedTarget)
        {
            delayedTarget = targetRatio;
            delayedTimer = 0f;
            delayedFollowing = false;
            delayedVelocity = 0f;

            delayedHealthSlider.normalizedValue =
                targetRatio;

            return;
        }

        // 피해
        delayedTarget = targetRatio;

        // 이미 회색바가 따라가는 중이면 딜레이를 다시 시작하지 않음
        if (delayedFollowing)
            return;

        delayedTimer = delayedHealthDelay;
        delayedFollowing = true;
    }

    private void HandleShieldChanged(double currentShield)
    {
        UpdateHealthText(
            health.CurrentHealth,
            health.MaxHealth,
            currentShield
        );
        double maxHealth = health.MaxHealth;
        double currentHealth = health.CurrentHealth;

        double barMax =
            GetBarMax(
                currentHealth,
                maxHealth,
                currentShield
            );

        float targetRatio =
            barMax <= 0d
                ? 0f
                : (float)Math.Max(
                    0d,
                    Math.Min(1d, currentHealth / barMax)
                );

        UpdateShieldSlider(
            currentHealth,
            currentShield,
            barMax
        );

        if (!initialized)
        {
            SnapTo(
                targetRatio,
                shieldTarget
            );

            initialized = true;
            lastMaxHealth = maxHealth;
            return;
        }

        // 현재 체력 비율과 실드 비율을 부드럽게 변경
        currentTarget = targetRatio;
        currentVelocity = 0f;

        shieldVelocity = 0f;

        // 실드 변화는 체력 피해 버퍼를 만들지 않음
        delayedTarget = targetRatio;
        delayedTimer = 0f;
        delayedFollowing = true;
        delayedVelocity = 0f;

        lastMaxHealth = maxHealth;
    }
    private void UpdateHealthText(
        double currentHealth,
        double maxHealth,
        double currentShield)
    {
        string currentHealthText =
            GameFormatUtils.ToIdleNumber(currentHealth);

        string shieldText =
            currentShield > 0d
                ? $"(<color=#4DA6FF>+{GameFormatUtils.ToIdleNumber(currentShield)}</color>) "
                : string.Empty;

        string maxHealthText =
            GameFormatUtils.ToIdleNumber(maxHealth);

        healthText.text =
            $"{currentHealthText}{shieldText}/ " +
            $"{maxHealthText}";
    }

    private double GetBarMax(
        double currentHealth,
        double maxHealth,
        double currentShield)
    {
        if (!health.IsShieldEnabled)
            return maxHealth;

        return Math.Max(
            maxHealth,
            currentHealth + currentShield
        );
    }

    private void UpdateShieldSlider(
        double currentHealth,
        double currentShield,
        double barMax)
    {
        shieldHealthSlider.gameObject.SetActive(
            health.IsShieldEnabled
        );

        float shieldEndRatio =
            barMax <= 0d
                ? 0f
                : (float)Math.Max(
                    0d,
                    Math.Min(
                        1d,
                        (currentHealth + currentShield) / barMax
                    )
                );

        shieldTarget = shieldEndRatio;
    }

    // 애님 없이 즉시 변경
    private void SnapTo(
        float targetRatio,
        float targetShieldRatio)
    {
        currentTarget = targetRatio;
        delayedTarget = targetRatio;
        shieldTarget = targetShieldRatio;

        currentVelocity = 0f;
        delayedVelocity = 0f;
        shieldVelocity = 0f;

        delayedTimer = 0f;
        delayedFollowing = false;

        currentHealthSlider.normalizedValue =
            targetRatio;

        delayedHealthSlider.normalizedValue =
            targetRatio;

        shieldHealthSlider.normalizedValue =
            targetShieldRatio;
    }
}

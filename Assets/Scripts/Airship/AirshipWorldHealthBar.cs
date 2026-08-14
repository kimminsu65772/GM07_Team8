using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AirshipWorldHealthBar : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private AirshipHealth health;
    [SerializeField] private Slider currentHealthSlider;
    [SerializeField] private Slider delayedHealthSlider;
    [SerializeField] private TMP_Text healthText;

    [Header("연출")]
    [SerializeField] private float currentSmoothTime = 0.1f;
    [SerializeField] private float delayedHealthDelay = 0.4f;
    [SerializeField] private float delayedSmoothTime = 0.2f;

    private float currentTarget;
    private float delayedTarget;

    private float currentVelocity;
    private float delayedVelocity;

    private float delayedTimer;

    private bool delayedFollowing;
    // 첫 체력 설정 시 애님 없이 바로 초기화
    private bool initialized;
    // 최대 체력 변경 감지용 변수. 변경시 애님없이 재설정.
    private float lastMaxHealth = -1f;

    private void OnEnable()
    {
        health.OnHealthChanged += HandleHealthChanged;

        HandleHealthChanged(
            health.CurrentHealth,
            health.MaxHealth
        );
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= HandleHealthChanged;
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

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        healthText.text =
            $"{Mathf.RoundToInt(currentHealth)} / {Mathf.RoundToInt(maxHealth)}";
        
        float targetRatio =
            maxHealth <= 0f
                ? 0f
                : Mathf.Clamp01(currentHealth / maxHealth);

        // 최초 초기화 또는 최대 체력 변경
        // 소수 비교 조심할 것.
        if (!initialized || !Mathf.Approximately(lastMaxHealth, maxHealth))
        {
            SnapTo(targetRatio);

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

    // 애님없이 즉시 변경하는 함수.
    private void SnapTo(float targetRatio)
    {
        currentTarget = targetRatio;
        delayedTarget = targetRatio;

        currentVelocity = 0f;
        delayedVelocity = 0f;

        delayedTimer = 0f;
        delayedFollowing = false;

        currentHealthSlider.normalizedValue =
            targetRatio;

        delayedHealthSlider.normalizedValue =
            targetRatio;
    }
}
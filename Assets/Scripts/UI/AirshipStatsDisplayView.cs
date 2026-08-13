using TMPro;
using UnityEngine;

public class AirshipStatsDisplayView : MonoBehaviour
{
    [Header("비행선 컨트롤러 참조")]
    [SerializeField] private AirshipController airshipController;

    [Header("UI 텍스트 연결 (레벨 / 수치)")]
    [SerializeField] private TextMeshProUGUI attackLevelText;
    [SerializeField] private TextMeshProUGUI attackValueText;

    [SerializeField] private TextMeshProUGUI defenseLevelText;
    [SerializeField] private TextMeshProUGUI defenseValueText;

    [SerializeField] private TextMeshProUGUI maxHealthLevelText;
    [SerializeField] private TextMeshProUGUI maxHealthValueText;

    [SerializeField] private TextMeshProUGUI criticalLevelText;
    [SerializeField] private TextMeshProUGUI criticalValueText;

    private void Start()
    {
        if (airshipController == null)
        {
            airshipController = FindFirstObjectByType<AirshipController>();
        }

        if (airshipController != null)
        {
            airshipController.UpgradeController.OnUpgradeChanged += HandleUpgradeChanged;
            HandleUpgradeChanged(airshipController.UpgradeController.UpgradeState);
        }
        else
        {
            Debug.LogWarning("AirshipStatsDisplayView: 씬에서 AirshipController를 찾을 수 없습니다!");
        }
    }
    private void OnDestroy()
    {
        if (airshipController != null && airshipController.UpgradeController != null)
        {
            airshipController.UpgradeController.OnUpgradeChanged -= HandleUpgradeChanged;
        }
    }
    private void HandleUpgradeChanged(AirshipUpgradeState state)
    {
        if (state == null) return;

        UpdateStatUI(AirshipStatType.Attack, state.AttackLevel, attackLevelText, attackValueText);
        UpdateStatUI(AirshipStatType.Defense, state.DefenseLevel, defenseLevelText, defenseValueText);
        UpdateStatUI(AirshipStatType.MaxHealth, state.MaxHealthLevel, maxHealthLevelText, maxHealthValueText);
        UpdateStatUI(AirshipStatType.CriticalChance, state.CriticalLevel, criticalLevelText, criticalValueText);
    }
    private void UpdateStatUI(AirshipStatType statType, int level, TextMeshProUGUI levelText, TextMeshProUGUI valueText)
    {
        if (levelText != null)
        {
            levelText.text = $"LV.{level}";
        }

        if (valueText != null)
        {
            float power = GetStatPower(statType, level);

            if (statType == AirshipStatType.CriticalChance)
            {
                valueText.text = $"{power * 100f:F1}%";
            }
            else
            {
                valueText.text = $"{power:F1}";
            }
        }
    }
    private float GetStatPower(AirshipStatType statType, int level)
    {
        switch (statType)
        {
            case AirshipStatType.Attack: return 10f + (level - 1) * 2f;
            case AirshipStatType.Defense: return 1f + (level - 1) * 1f;
            case AirshipStatType.MaxHealth: return 100f + (level - 1) * 20f;
            case AirshipStatType.CriticalChance: return 0.05f + (level - 1) * 0.01f;
            default: return 0f;
        }
    }
}
using TMPro;
using UnityEngine;

public class AirshipStatsDisplayView : MonoBehaviour
{
    [Header("UI 텍스트 연결 (최종 수치)")]
    [SerializeField] private TextMeshProUGUI attackValueText;
    [SerializeField] private TextMeshProUGUI defenseValueText;
    [SerializeField] private TextMeshProUGUI maxHealthValueText;
    [SerializeField] private TextMeshProUGUI criticalValueText;
    private AirshipStatController statController;
    private void Start()
    {
        statController = FindFirstObjectByType<AirshipStatController>();
        if (statController != null)
        {
            statController.OnStatsChanged += HandleStatsChanged;
            HandleStatsChanged(statController.CurrentStats);
        }
        else
        {
            Debug.LogWarning("AirshipStatsDisplayView: 씬에서 AirshipStatController를 찾을 수 없습니다!");
        }
    }

    private void OnDestroy()
    {
        if (statController != null)
        {
            statController.OnStatsChanged -= HandleStatsChanged;
        }
    }
    private void HandleStatsChanged(AirshipRuntimeStats stats)
    {
        if (stats == null) return;

        UpdateStatUI(AirshipStatType.Attack, stats.Attack, attackValueText);
        UpdateStatUI(AirshipStatType.Defense, stats.Defense, defenseValueText);
        UpdateStatUI(AirshipStatType.MaxHealth, stats.MaxHealth, maxHealthValueText);
        UpdateStatUI(AirshipStatType.CriticalChance, stats.CriticalChance, criticalValueText);
    }
    private void UpdateStatUI(AirshipStatType statType, float finalValue, TextMeshProUGUI valueText)
    {
        if (valueText == null) return;

        if (statType == AirshipStatType.CriticalChance)
        {
            valueText.text = $"{finalValue * 100f:F1}%";
        }
        else
        {
            valueText.text = $"{finalValue:F1}";
        }
    }
}
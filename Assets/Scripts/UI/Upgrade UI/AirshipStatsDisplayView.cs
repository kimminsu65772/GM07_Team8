using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AirshipStatsDisplayView : MonoBehaviour
{
    [Header("UI 텍스트 연결 (최종 수치)")]
    [SerializeField] private TextMeshProUGUI attackValueText;
    [FormerlySerializedAs("defenseValueText")] [SerializeField] private TextMeshProUGUI recoveryValueText;
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
        UpdateStatUI(AirshipStatType.Recovery, stats.Recovery, recoveryValueText);
        UpdateStatUI(AirshipStatType.MaxHealth, stats.MaxHealth, maxHealthValueText);
        UpdateStatUI(AirshipStatType.CriticalChance, stats.CriticalChance, criticalValueText);
    }
    private void UpdateStatUI(AirshipStatType statType, double finalValue, TextMeshProUGUI valueText)
    {
        if (valueText == null) return;

        if (statType == AirshipStatType.CriticalChance)
        {
            valueText.text = $"{finalValue * 100f:F1}%";
        }
        else
        {
            valueText.text = GameFormatUtils.ToIdleNumber(finalValue);
        }
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    [SerializeField] private AirshipStatType statType;
    [SerializeField] private TextMeshProUGUI nameLevelText;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button upgradeButton;

    private AirshipUpgradeController controller;
    public void Init(AirshipUpgradeController controller)
    {
        this.controller = controller;

        // 중복 등록 방지를 위해 리스너 제거 후 추가
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
    }
    private void OnUpgradeButtonClicked()
    {
        if (controller.TryUpgrade(statType))
        {
            Debug.Log($"{statType} 업그레이드 성공!");
        }
        else
        {
            Debug.Log("재화가 부족하거나 최대 레벨입니다.");
        }
    }
    public void RefreshUI(AirshipUpgradeState state)
    {
        bool isMax = controller.IsMaxLevel(statType);
        int currentLevel = controller.GetCurrentLevel(statType);

        if (isMax)
        {
            nameLevelText.text = $"{statType} MAX";
            costText.text = "MAX";
            upgradeButton.interactable = false;
        }
        else
        {
            nameLevelText.text = $"{statType} LV.{currentLevel}";
            costText.text = controller.GetCost(statType).ToString("N0");
            upgradeButton.interactable = true;
        }
        float currentStat = controller.GetCurrentStat(statType);

        if (isMax)
        {
            string formattedCurrent = GameFormatUtils.FormatStatValue(statType, currentStat);
            if (statValueText != null)
            {
                statValueText.text = $"{formattedCurrent} (최대)";
            }
        }
        else
        {
            float nextStat = controller.GetNextStat(statType);
            string formattedCurrent = GameFormatUtils.FormatStatValue(statType, currentStat);
            string formattedNext = GameFormatUtils.FormatStatValue(statType, nextStat);

            if (statValueText != null)
            {
                statValueText.text = $"{formattedCurrent} -> <color=#00FF00>{formattedNext}</color>";
            }
        }
    }
}
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

    [SerializeField] private UpgradeToggleUI upgradeToggleUI;

    private ButtonHoverScale hoverScaleComponent;
    private ButtonSoundPlayer soundPlayerComponent;

    private AirshipUpgradeController controller;
    private void Awake()
    {
        if (upgradeButton != null)
        {
            hoverScaleComponent = upgradeButton.GetComponent<ButtonHoverScale>();
            soundPlayerComponent = upgradeButton.GetComponent<ButtonSoundPlayer>();
        }
    }
    private void OnEnable()
    {
        if (upgradeToggleUI != null)
        {
            upgradeToggleUI.OnModeChanged += HandleModeChanged;
        }
    }
    private void OnDisable()
    {
        if (upgradeToggleUI != null)
        {
            upgradeToggleUI.OnModeChanged -= HandleModeChanged;
        }
    }
    private void HandleModeChanged(int newMode)
    {
        if (controller != null)
        {
            RefreshUI(controller.UpgradeState);
        }
    }
    public void Init(AirshipUpgradeController controller)
    {
        this.controller = controller;

        // 중복 등록 방지를 위해 리스너 제거 후 추가
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
    }
    private void OnUpgradeButtonClicked()
    {
        int upgradeLevelCount = upgradeToggleUI.CurrentUpgradeMode;

        if (controller.TryUpgrade(statType, upgradeLevelCount))
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
        if (controller == null) return;
        bool isMax = controller.IsMaxLevel(statType);
        int currentLevel = controller.GetCurrentLevel(statType);
        int upgradeLevelCount = upgradeToggleUI.CurrentUpgradeMode;
        long cost = controller.GetUpgradeCost(statType, upgradeLevelCount);

        if (isMax)
        {
            nameLevelText.text = $"{statType} MAX";
            upgradeButton.interactable = false;
            if (costText != null) costText.text = "MAX";
            // MAX일 때 호버 스케일과 사운드 플레이어 끄기
            if (hoverScaleComponent != null) hoverScaleComponent.enabled = false;
            if (soundPlayerComponent != null) soundPlayerComponent.enabled = false;
        }
        else
        {
            nameLevelText.text = $"{statType} LV.{currentLevel}";
            upgradeButton.interactable = controller.CanAffordUpgrade(statType, upgradeLevelCount);
            if (costText != null) costText.text = $"{GameFormatUtils.ToIdleNumber(cost)}";

            // MAX가 아닐 때 호버 스케일과 사운드 플레이어 켜기
            if (hoverScaleComponent != null) hoverScaleComponent.enabled = true;
            if (soundPlayerComponent != null) soundPlayerComponent.enabled = true;
        }
        double currentStat = controller.GetCurrentStat(statType);

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
            double nextStat = controller.GetTargetStat(statType, upgradeLevelCount);
            string formattedCurrent = GameFormatUtils.FormatStatValue(statType, currentStat);
            string formattedNext = GameFormatUtils.FormatStatValue(statType, nextStat);

            if (statValueText != null)
            {
                statValueText.text = $"{formattedCurrent} -> <color=#00FF00>{formattedNext}</color>";
            }
        }
    }
}
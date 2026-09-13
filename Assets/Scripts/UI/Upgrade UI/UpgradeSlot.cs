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

        if (upgradeButton != null)
        {
            // RemoveAllListeners 대신 기존에 등록된 내 함수만 제거 후 다시 추가 (사운드 리스너 보존)
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }
    }
    private void OnUpgradeButtonClicked()
    {
        if (controller == null || upgradeToggleUI == null) return;
        int upgradeLevelCount = upgradeToggleUI.CurrentUpgradeMode;

        // 실제로 업그레이드를 시도하는 로직
        if (controller.TryUpgrade(statType, upgradeLevelCount)) { }

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
            nameLevelText.text = "MAX";
            upgradeButton.interactable = false;
            if (costText != null) costText.text = "MAX";
            // MAX일 때 호버 스케일과 사운드 플레이어 끄기
            if (hoverScaleComponent != null) hoverScaleComponent.enabled = false;
            if (soundPlayerComponent != null) soundPlayerComponent.enabled = false;
        }
        else
        {
            nameLevelText.text = $"LV.{currentLevel}";
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
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUIController : MonoBehaviour
{
    [SerializeField] private AirshipUpgradeController upgradeController;
    [SerializeField] private UpgradeToggleUI toggleUI;
    [SerializeField] private List<UpgradeSlot> slots;

    private void OnEnable()
    {
        if (toggleUI != null)
        {
            toggleUI.OnModeChanged += HandleModeChanged;
        }
    }

    private void OnDisable()
    {
        if (toggleUI != null)
        {
            toggleUI.OnModeChanged -= HandleModeChanged;
        }
    }
    private void Start()
    {
        if (upgradeController == null)
        {
            return;
        }
        upgradeController.Init();

        foreach (var slot in slots)
        {
            slot.Init(upgradeController);
        }

        upgradeController.OnUpgradeChanged += RefreshAllSlots;
        if (PlayerInfo.Instance != null)
        {
            PlayerInfo.Instance.OnCurrencyChanged += HandleCurrencyChanged;
        }
        if (toggleUI != null)
        {
            toggleUI.OnModeChanged += HandleModeChanged;
        }
        RefreshAllSlots(upgradeController.UpgradeState);
    }
    private void OnDestroy()
    {
        if (upgradeController != null)
        {
            upgradeController.OnUpgradeChanged -= RefreshAllSlots;
        }
        if (PlayerInfo.Instance != null)
        {
            PlayerInfo.Instance.OnCurrencyChanged -= HandleCurrencyChanged;
        }
    }
    private void RefreshAllSlots(AirshipUpgradeState state)
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                slot.RefreshUI(state);
            }
        }
    }
    private void HandleModeChanged(int newMode)
    {
        RefreshAllSlots(upgradeController.UpgradeState);
    }
    private void HandleCurrencyChanged(
        CurrencyType changedCurrencyType)
    {
        if (upgradeController == null ||
            changedCurrencyType != upgradeController.UpgradeCurrency)
        {
            return;
        }

        RefreshAllSlots(upgradeController.UpgradeState);
    }
}
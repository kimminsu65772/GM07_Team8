using System.Collections.Generic;
using UnityEngine;

public class UpgradeUIController : MonoBehaviour
{
    [SerializeField] private AirshipUpgradeController upgradeController;
    [SerializeField] private List<UpgradeSlot> slots;

    private void Start()
    {
        if (upgradeController == null)
        {
            Debug.LogError("UpgradeUIController: upgradeController가 연결되지 않았습니다!");
            return;
        }
        upgradeController.Init();

        foreach (var slot in slots)
        {
            slot.Init(upgradeController);
        }

        upgradeController.OnUpgradeChanged += RefreshAllSlots;
        PlayerInfo.Instance.OnCurrencyChanged += HandleCurrencyChanged;
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
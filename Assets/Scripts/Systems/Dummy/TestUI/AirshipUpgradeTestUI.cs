using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirshipUpgradeTestUI : MonoBehaviour
{
    [SerializeField] private AirshipUpgradeController upgradeController;

    [Header("Upgrade Buttons")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button defenseButton;
    [SerializeField] private Button maxHealthButton;
    [SerializeField] private Button criticalButton;

    [SerializeField] private TMP_Text resultText;

    private void OnEnable()
    {
        AddButtonListeners();

        if (upgradeController == null)
        {
            SetButtonsInteractable(false);
            SetResult("AirshipUpgradeController is not assigned.");
            return;
        }

        SetButtonsInteractable(true);
        upgradeController.OnUpgradeChanged += HandleUpgradeChanged;
        RefreshButtonLabels(upgradeController.UpgradeState);
    }

    private void OnDisable()
    {
        RemoveButtonListeners();

        if (upgradeController != null)
        {
            upgradeController.OnUpgradeChanged -= HandleUpgradeChanged;
        }
    }

    private void AddButtonListeners()
    {
        if (attackButton != null)
            attackButton.onClick.AddListener(UpgradeAttack);
        if (defenseButton != null)
            defenseButton.onClick.AddListener(UpgradeDefense);
        if (maxHealthButton != null)
            maxHealthButton.onClick.AddListener(UpgradeMaxHealth);
        if (criticalButton != null)
            criticalButton.onClick.AddListener(UpgradeCritical);
    }

    private void RemoveButtonListeners()
    {
        if (attackButton != null)
            attackButton.onClick.RemoveListener(UpgradeAttack);
        if (defenseButton != null)
            defenseButton.onClick.RemoveListener(UpgradeDefense);
        if (maxHealthButton != null)
            maxHealthButton.onClick.RemoveListener(UpgradeMaxHealth);
        if (criticalButton != null)
            criticalButton.onClick.RemoveListener(UpgradeCritical);
    }

    private void UpgradeAttack()
    {
        TryUpgrade(AirshipStatType.Attack);
    }

    private void UpgradeDefense()
    {
        TryUpgrade(AirshipStatType.Defense);
    }

    private void UpgradeMaxHealth()
    {
        TryUpgrade(AirshipStatType.MaxHealth);
    }

    private void UpgradeCritical()
    {
        TryUpgrade(AirshipStatType.CriticalChance);
    }

    private void TryUpgrade(AirshipStatType statType)
    {
        if (upgradeController == null)
        {
            return;
        }

        bool success = upgradeController.TryUpgrade(statType);
        SetResult(success
            ? $"{statType} upgrade succeeded."
            : $"{statType} upgrade failed.");
    }

    private void HandleUpgradeChanged(AirshipUpgradeState state)
    {
        RefreshButtonLabels(state);
    }

    private void RefreshButtonLabels(AirshipUpgradeState state)
    {
        if (state == null)
        {
            return;
        }

        SetButtonLabel(attackButton, $"Attack Lv.{state.AttackLevel}");
        SetButtonLabel(defenseButton, $"Defense Lv.{state.DefenseLevel}");
        SetButtonLabel(maxHealthButton, $"Max Health Lv.{state.MaxHealthLevel}");
        SetButtonLabel(criticalButton, $"Critical Lv.{state.CriticalLevel}");
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (attackButton != null) attackButton.interactable = interactable;
        if (defenseButton != null) defenseButton.interactable = interactable;
        if (maxHealthButton != null) maxHealthButton.interactable = interactable;
        if (criticalButton != null) criticalButton.interactable = interactable;
    }

    private void SetButtonLabel(Button button, string label)
    {
        if (button == null)
        {
            return;
        }

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>(true);
        if (buttonText != null)
        {
            buttonText.text = label;
        }
    }

    private void SetResult(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
        }
    }
}

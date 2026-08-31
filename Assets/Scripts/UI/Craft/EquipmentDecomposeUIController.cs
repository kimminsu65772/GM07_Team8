using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDecomposeUIController : MonoBehaviour
{
    [Header("결과 패널")]
    [SerializeField] private DecomposeResultPanelUI resultPanelUI;

    [Header("버튼")]
    [SerializeField] private Button confirmDecomposeButton;

    private readonly List<EquipmentSaveData> selectedEquipments = new();
    private readonly Dictionary<int, EquipmentSaveData> selectedEquipmentById = new();
    public IReadOnlyList<EquipmentSaveData> SelectedEquipments => selectedEquipments;

    private void OnEnable()
    {
        if (confirmDecomposeButton != null)
        {
            confirmDecomposeButton.onClick.RemoveListener(OnDecomposeButtonClicked);
            confirmDecomposeButton.onClick.AddListener(OnDecomposeButtonClicked);
        }

        if (resultPanelUI != null)
        {
            resultPanelUI.Hide();
        }

        RefreshDecomposeButton();
    }

    private void OnDisable()
    {
        if (confirmDecomposeButton != null)
        {
            confirmDecomposeButton.onClick.RemoveListener(OnDecomposeButtonClicked);
        }
    }

    public bool ToggleEquipment(EquipmentSaveData equipment)
    {
        if (equipment == null)
        {
            return false;
        }

        if (IsSelected(equipment.EquipId))
        {
            selectedEquipmentById.Remove(equipment.EquipId);
            RemoveSelectedEquipment(equipment.EquipId);
            RefreshDecomposeButton();
            return false;
        }

        selectedEquipments.Add(equipment);
        selectedEquipmentById.Add(equipment.EquipId, equipment);

        RefreshDecomposeButton();
        return true;
    }

    public bool IsSelected(int equipId)
    {
        return selectedEquipmentById.ContainsKey(equipId);
    }

    private void RemoveSelectedEquipment(int equipId)
    {
        for (int i = 0; i < selectedEquipments.Count; i++)
        {
            if (selectedEquipments[i] != null &&
                selectedEquipments[i].EquipId == equipId)
            {
                selectedEquipments.RemoveAt(i);
                return;
            }
        }
    }

    public void ClearSelection()
    {
        selectedEquipments.Clear();
        selectedEquipmentById.Clear();

        RefreshDecomposeButton();
    }

    private void RefreshDecomposeButton()
    {
        if (confirmDecomposeButton == null)
        {
            return;
        }

        confirmDecomposeButton.interactable = selectedEquipments.Count > 0;
    }

    private void OnDecomposeButtonClicked()
    {
        if (selectedEquipments.Count == 0)
        {
            RefreshDecomposeButton();
            return;
        }

        Dictionary<int, int> rewards = EquipmentDecomposeCalculator.GenerateTotalRewards(selectedEquipments);

        foreach (EquipmentSaveData equipment in selectedEquipments)
        {
            if (equipment == null)
            {
                continue;
            }

            PlayerInfo.Instance.RemoveEquipment(equipment.EquipId, SavePolicy.Deferred);
        }

        foreach ((int itemId, int amount) in rewards)
        {
            PlayerInfo.Instance.AddItem(itemId, amount, SavePolicy.Soon);
        }

        ClearSelection();

        if (resultPanelUI != null)
        {
            resultPanelUI.Show(rewards);
        }
    }
}

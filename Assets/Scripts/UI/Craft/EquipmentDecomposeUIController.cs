using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDecomposeUIController : MonoBehaviour
{
    [SerializeField] private EquipmentDB equipmentDB;
    [SerializeField] private HeroEquipmentSlot[] decomposeSlots;

    [Header("결과 패널")]
    [SerializeField] private DecomposeResultPanelUI resultPanelUI;

    [Header("버튼")]
    [SerializeField] private Button decomposeButton;

    private readonly List<EquipmentSaveData> selectedEquipments = new();
    private readonly Dictionary<int, EquipmentSaveData> selectedEquipmentById = new();
    public IReadOnlyList<EquipmentSaveData> SelectedEquipments => selectedEquipments;

    private void OnEnable()
    {
        if (decomposeButton != null)
        {
            decomposeButton.onClick.RemoveListener(OnDecomposeButtonClicked);
            decomposeButton.onClick.AddListener(OnDecomposeButtonClicked);
        }

        if (resultPanelUI != null)
        {
            resultPanelUI.Hide();
        }

        RefreshSlots();
    }

    private void OnDisable()
    {
        if (decomposeButton != null)
        {
            decomposeButton.onClick.RemoveListener(OnDecomposeButtonClicked);
        }
    }

    public bool ToggleEquipment(EquipmentSaveData equipment)
    {
        if (equipment == null)
        {
            return false;
        }

        if (selectedEquipmentById.ContainsKey(equipment.EquipId))
        {
            selectedEquipmentById.Remove(equipment.EquipId);
            RemoveSelectedEquipment(equipment.EquipId);
            RefreshSlots();
            return true;
        }

        if (decomposeSlots == null ||
            selectedEquipments.Count >= decomposeSlots.Length)
        {
            Debug.LogWarning("모든 슬롯이 가득 찼습니다.");
            return false;
        }

        selectedEquipments.Add(equipment);
        selectedEquipmentById.Add(equipment.EquipId, equipment);

        RefreshSlots();
        return true;
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
        if (decomposeSlots == null) return;

        selectedEquipments.Clear();
        selectedEquipmentById.Clear();

        RefreshSlots();
    }

    private void RefreshSlots()
    {
        if (decomposeSlots == null) return;

        for (int i = 0; i < decomposeSlots.Length; i++)
        {
            HeroEquipmentSlot slot = decomposeSlots[i];

            if (slot == null) continue;

            if (i >= selectedEquipments.Count)
            {
                slot.SetClickAction(null);
                slot.ClearSlot();
                continue;
            }

            EquipmentSaveData equipment = selectedEquipments[i];

            if (equipment == null)
            {
                slot.SetClickAction(null);
                slot.ClearSlot();
                continue;
            }

            EquipmentSO equipmentSO = equipmentDB != null ? equipmentDB.GetEquipmentSO(equipment.EquipDataId) : null;

            slot.SetSlot(equipment, equipmentSO, false);
            slot.SetClickAction(OnDecomposeSlotClicked);
        }
        RefreshDecomposeButton();
    }
    private void RefreshDecomposeButton()
    {
        if (decomposeButton == null)
        {
            return;
        }

        decomposeButton.interactable = selectedEquipments.Count > 0;
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

    private void OnDecomposeSlotClicked(HeroEquipmentSlot slot)
    {
        if (slot == null)
        {
            return;
        }

        EquipmentSaveData equipment = slot.EquipmentSaveData;

        if (equipment == null)
        {
            return;
        }

        ToggleEquipment(equipment);
    }
}

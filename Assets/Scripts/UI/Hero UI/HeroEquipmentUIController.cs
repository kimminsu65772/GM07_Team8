using UnityEngine;

public class HeroEquipmentUIController : MonoBehaviour
{
    [Header("영웅 장비 슬롯 연결")]
    [SerializeField] private HeroEquipmentSlot weaponSlot;
    [SerializeField] private HeroEquipmentSlot bodySlot;
    [SerializeField] private HeroEquipmentSlot accSlot;

    [Header("영웅 UI 컨트롤러")]
    [SerializeField] private HeroInventoryUIController heroInventoryUIController;

    [Header("장비 정보")]
    [SerializeField] private EquipmentDB equipmentDB;


    private void OnEnable()
    {
        if (heroInventoryUIController == null) return;

        heroInventoryUIController.OnHeroSelected -= RefreshSlots;
        heroInventoryUIController.OnHeroSelected += RefreshSlots;

        RefreshSlots(heroInventoryUIController.SelectecHeroEntry, heroInventoryUIController.SelectedHeroSaveData);
    }
    private void OnDisable()
    {
        if (heroInventoryUIController == null) return;
        heroInventoryUIController.OnHeroSelected -= RefreshSlots;
    }

    private void RefreshSlots(HeroEntry heroEntry, HeroSaveData heroSaveData)
    {
        if (equipmentDB == null)
        {
            ClearSlots();
            return;
        }
        if (heroEntry == null || heroSaveData == null)
        {
            ClearSlots();
            return;
        }

        PlayerInfo.Instance.GetHeroEquippedEquipments(
            heroEntry.HeroId,
            out EquipmentSaveData weapon,
            out EquipmentSaveData body,
            out EquipmentSaveData acc
        );

        SetSlot(weaponSlot, weapon);
        SetSlot(bodySlot, body);
        SetSlot(accSlot, acc);
    }

    private void SetSlot(HeroEquipmentSlot slot, EquipmentSaveData saveData)
    {
        if (slot == null)
        {
            return;
        }

        if (saveData == null)
        {
            slot.ClearSlot();
            return;
        }

        EquipmentSO equipmentSO = equipmentDB.GetEquipmentSO(saveData.EquipDataId);
        slot.SetSlot(saveData, equipmentSO, false);
    }
    private void ClearSlots()
    {
        if (weaponSlot != null) weaponSlot.ClearSlot();
        if (bodySlot != null) bodySlot.ClearSlot();
        if (accSlot != null) accSlot.ClearSlot();
    }
}

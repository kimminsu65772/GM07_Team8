using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftCompleteUIController : MonoBehaviour
{
    [SerializeField] private HeroEquipmentSlot heroEquipmentSlot;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private Button closeButton;

    [Header("등급 별 텍스트 색상 설정")]
    [SerializeField] private Color commonColor;
    [SerializeField] private Color rareColor;
    [SerializeField] private Color epicColor;
    [SerializeField] private Color legendaryColor;

    public void OnEnable()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Hide);
        }
    }

    public void OnDisable()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Hide);
        }
    }

    public void SetCraftedItem(EquipmentSaveData saveData, EquipmentSO equipmentData, bool isEquipped)
    {
        gameObject.SetActive(true);
        Clear();

        if (heroEquipmentSlot != null)
        {
            heroEquipmentSlot.SetSlot(saveData, equipmentData, isEquipped);
        }
        if (itemNameText != null && equipmentData != null)
        {
            itemNameText.text = equipmentData.EquipName;
            SetItemNameColor(equipmentData);
        }
    }

    public void Hide()
    {
        Clear();
        gameObject.SetActive(false);
    }

    private void Clear()
    {
        if (heroEquipmentSlot != null)
        {
            heroEquipmentSlot.ClearSlot();
        }
        if (itemNameText != null)
        {
            itemNameText.text = string.Empty;
            itemNameText.color = commonColor;
        }
    }

    private void SetItemNameColor(EquipmentSO equipmentData)
    {
        if (itemNameText != null && equipmentData != null)
        {
            Color color = equipmentData.EquipGrade switch
            {
                EquipGradeEnum.Common => commonColor,
                EquipGradeEnum.Rare => rareColor,
                EquipGradeEnum.Epic => epicColor,
                EquipGradeEnum.Legendary => legendaryColor,
                _ => Color.white
            };
            itemNameText.color = color;
        }
    }
}

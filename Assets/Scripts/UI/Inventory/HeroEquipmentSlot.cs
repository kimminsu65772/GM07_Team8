using System;
using UnityEngine;
using UnityEngine.UI;

public class HeroEquipmentSlot : MonoBehaviour
{
    [SerializeField] private Image gradeFrame;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image equipBadge;

    [Header("슬롯 용도 설정")]
    [SerializeField] private EquipPartEnum equipPart;

    [Header("등급 별 색상 설정")]
    [SerializeField] private Color commonColor;
    [SerializeField] private Color rareColor;
    [SerializeField] private Color epicColor;
    [SerializeField] private Color legendaryColor;

    private EquipmentSaveData equipmentSaveData;
    private EquipmentSO equipmentSO;

    private Button slotBtn;
    private event Action<HeroEquipmentSlot> onClickButtonEvent;

    public EquipmentSaveData EquipmentSaveData => equipmentSaveData;
    public EquipmentSO EquipmentSO => equipmentSO;
    public EquipPartEnum EquipPart => equipPart;

    private void Awake()
    {
        gameObject.TryGetComponent(out Button button);
        if (button == null)
        {
            slotBtn = gameObject.AddComponent<Button>();
        }
        else
        {
            slotBtn = button;
        }
    }

    private void OnEnable()
    {
        slotBtn.onClick.AddListener(OnSlotClicked);
    }

    private void OnDisable()
    {
        slotBtn.onClick.RemoveListener(OnSlotClicked);
    }

    public void SetSlot(EquipmentSaveData saveData, EquipmentSO equipmentData, bool isEquipped)
    {
        equipmentSaveData = saveData;
        equipmentSO = equipmentData;

        if (saveData == null && equipmentData == null)
        {
            ClearSlot();
            return;
        }

        EquipGradeEnum grade = equipmentData != null
            ? equipmentData.EquipGrade
            : saveData.EquipGrade;

        SetGradeColor(grade);
        SetIcon(equipmentData);
        SetEquipped(isEquipped);
    }

    public void SetEquipped(bool isEquipped)
    {
        if (equipBadge != null)
        {
            equipBadge.gameObject.SetActive(isEquipped);
        }
    }

    public void ClearSlot()
    {
        equipmentSaveData = null;
        equipmentSO = null;

        if (gradeFrame != null)
        {
            gradeFrame.color = commonColor;
        }

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        SetEquipped(false);
    }

    public void SetClickAction(Action<HeroEquipmentSlot> onClickAction)
    {
        onClickButtonEvent = onClickAction;
    }

    private void OnSlotClicked()
    {
        if (onClickButtonEvent != null)
        {
            onClickButtonEvent.Invoke(this);
        }
    }

    private void SetIcon(EquipmentSO equipmentData)
    {
        if (iconImage == null)
        {
            return;
        }

        Sprite icon = equipmentData != null ? equipmentData.EquipIcon : null;
        iconImage.sprite = icon;
        iconImage.enabled = icon != null;
    }

    private void SetGradeColor(EquipGradeEnum grade)
    {
        if (gradeFrame == null)
        {
            return;
        }

        gradeFrame.color = GetGradeColor(grade);
    }

    private Color GetGradeColor(EquipGradeEnum grade)
    {
        return grade switch
        {
            EquipGradeEnum.Common => commonColor,
            EquipGradeEnum.Rare => rareColor,
            EquipGradeEnum.Epic => epicColor,
            EquipGradeEnum.Legendary => legendaryColor,
            _ => commonColor
        };
    }
}

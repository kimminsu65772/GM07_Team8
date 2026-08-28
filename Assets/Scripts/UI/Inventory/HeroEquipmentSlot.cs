using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroEquipmentSlot : MonoBehaviour
{
    [SerializeField] private Image gradeFrame;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image equipBadge;

    [Header("장비 텍스트")]
    [SerializeField] private TMP_Text equipmentNameText;
    [SerializeField] private TMP_Text equipmentStatText;

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

        Color gradeColor = GetGradeColor(grade);

        SetGradeColor(grade);
        SetIcon(equipmentData);
        SetName(equipmentData, gradeColor);
        SetStatText(saveData);
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

        if (equipmentNameText != null)
        {
            equipmentNameText.text = "";
            equipmentNameText.color = commonColor;
        }

        if (equipmentStatText != null)
        {
            equipmentStatText.text = "";
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

    private void SetName(EquipmentSO equipment, Color gradeColor)
    {
        if (equipmentNameText == null)
        {
            return;
        }

        equipmentNameText.text = equipment != null ? equipment.EquipName : "";
        equipmentNameText.color = gradeColor;
    }

    private void SetStatText(EquipmentSaveData saveData)
    {
        if (equipmentStatText == null)
        {
            return;
        }

        if (saveData == null)
        {
            equipmentStatText.text = "";
            return;
        }

        StringBuilder builder = new();

        string formattedHP = GameFormatUtils.ToIdleNumber(saveData.BonusHP);
        string formattedATK = GameFormatUtils.ToIdleNumber(saveData.BonusAtk);
        string formattedDEF = GameFormatUtils.ToIdleNumber(saveData.BonusDef);
        string formattedCRI = GameFormatUtils.ToPercent((float)(saveData.BonusCriChance/100));

        AppendStat(builder, "HP", formattedHP);
        AppendStat(builder, "ATK", formattedATK);
        AppendStat(builder, "DEF", formattedDEF);
        AppendStat(builder, "CRI", formattedCRI);

        equipmentStatText.text = builder.ToString();
    }

    // string을 더하는 작업이기 때문에 StringBuilder를 사용해봤음.
    private void AppendStat(StringBuilder builder, string label, string statText, string suffix = "")
    {
        if (builder.Length > 0)
        {
            builder.AppendLine();
        }

        builder.Append($"{label} + {statText}{suffix}");
    }
}

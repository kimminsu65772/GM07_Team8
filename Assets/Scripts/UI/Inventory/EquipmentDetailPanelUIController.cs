using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDetailPanelUIController : MonoBehaviour
{
    [Header("상세 패널 상단")]
    [SerializeField] private Image equipIcon;
    [SerializeField] private TMP_Text equipName;
    [SerializeField] private TMP_Text equippedHero;
                     
    [Header("상세 패널 중단")]
    [SerializeField] private TMP_Text equipStat;
                     
    [Header("상세 패널 하단")]
    [SerializeField] private TMP_Text equipDescription;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;

    private EquipmentSaveData currentEquipment;
    private EquipmentSO currentEquipmentSO;

    private HeroEntry selectedHeroEntry;
    private HeroSaveData selectedHeroSaveData;

    private void OnEnable()
    {
        if (equipButton != null)
        {
            equipButton.onClick.RemoveListener(OnEquipButtonClicked);
            equipButton.onClick.AddListener(OnEquipButtonClicked);
        }

        if (unequipButton != null)
        {
            unequipButton.onClick.RemoveListener(OnUnequipButtonClicked);
            unequipButton.onClick.AddListener(OnUnequipButtonClicked);
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (equipButton != null)
        {
            equipButton.onClick.RemoveListener(OnEquipButtonClicked);
        }

        if (unequipButton != null)
        {
            unequipButton.onClick.RemoveListener(OnUnequipButtonClicked);
        }
    }

    public void SetSelectedHero(HeroEntry heroEntry, HeroSaveData heroSaveData)
    {
        selectedHeroEntry = heroEntry;
        selectedHeroSaveData = heroSaveData;

        Refresh();
    }

    public void SetEquipment(EquipmentSaveData equipment, EquipmentSO equipmentSO)
    {
        currentEquipment = equipment;
        currentEquipmentSO = equipmentSO;

        Refresh();
    }

    public void Clear()
    {
        currentEquipment = null;
        currentEquipmentSO = null;

        Refresh();
    }

    private void Refresh()
    {
        if (currentEquipment == null)
        {
            ClearView();
            return;
        }

        bool isEquipped = PlayerInfo.Instance.TryGetEquippedHero(
            currentEquipment.EquipId,
            out HeroNameEnum equippedHeroId
        );

        SetIcon();
        SetName();
        SetStatText();
        SetDescription();
        SetEquippedHeroText(isEquipped, equippedHeroId);
        SetButtonState(isEquipped);
    }

    private void ClearView()
    {
        if (equipIcon != null)
        {
            equipIcon.sprite = null;
            equipIcon.enabled = false;
        }

        if (equipName != null)
        {
            equipName.text = "";
        }

        if (equippedHero != null)
        {
            equippedHero.text = "";
        }

        if (equipStat != null)
        {
            equipStat.text = "";
        }

        if (equipDescription != null)
        {
            equipDescription.text = "";
        }

        if (equipButton != null)
        {
            equipButton.gameObject.SetActive(false);
        }

        if (unequipButton != null)
        {
            unequipButton.gameObject.SetActive(false);
        }
    }

    private void SetIcon()
    {
        if (equipIcon == null)
        {
            return;
        }

        Sprite icon = currentEquipmentSO != null ? currentEquipmentSO.EquipIcon : null;
        equipIcon.sprite = icon;
        equipIcon.enabled = icon != null;
    }

    private void SetName()
    {
        if (equipName == null)
        {
            return;
        }

        equipName.text = currentEquipmentSO != null
            ? currentEquipmentSO.EquipName
            : "";
    }

    private void SetDescription()
    {
        if (equipDescription == null)
        {
            return;
        }

        equipDescription.text = currentEquipmentSO != null
            ? currentEquipmentSO.EquipDescription
            : "";
    }

    private void SetStatText()
    {
        if (equipStat == null)
        {
            return;
        }

        equipStat.text = BuildStatText(currentEquipment);
    }

    private void SetEquippedHeroText(bool isEquipped, HeroNameEnum equippedHeroId)
    {
        if (equippedHero == null)
        {
            return;
        }

        equippedHero.text = isEquipped
            ? $"장착 영웅: {equippedHeroId}"
            : "";
    }

    private void SetButtonState(bool isEquipped)
    {
        if (equipButton != null)
        {
            equipButton.gameObject.SetActive(!isEquipped);
            equipButton.interactable = selectedHeroEntry != null && selectedHeroSaveData != null;
        }

        if (unequipButton != null)
        {
            unequipButton.gameObject.SetActive(isEquipped);
        }
    }

    private void OnEquipButtonClicked()
    {
        if (currentEquipment == null)
        {
            return;
        }

        if (selectedHeroEntry == null || selectedHeroSaveData == null)
        {
            return;
        }

        bool result = PlayerInfo.Instance.SetHeroEquippedEquipmentId(
            selectedHeroEntry.HeroId,
            currentEquipment.EquipPart,
            currentEquipment.EquipId
        );

        if (!result)
        {
            return;
        }

        SoundManager.Instance.PlaySound(SoundId.EquipmentEquip);
        Refresh();
    }

    private void OnUnequipButtonClicked()
    {
        if (currentEquipment == null)
        {
            return;
        }

        if (!PlayerInfo.Instance.TryGetEquippedHero(currentEquipment.EquipId, out HeroNameEnum equippedHeroId))
        {
            Refresh();
            return;
        }

        bool result = PlayerInfo.Instance.ClearHeroEquippedEquipmentId(
            equippedHeroId,
            currentEquipment.EquipPart
        );

        if (!result)
        {
            return;
        }

        SoundManager.Instance.PlaySound(SoundId.EquipmentUnequip);
        Refresh();
    }

    private string BuildStatText(EquipmentSaveData saveData)
    {
        if (saveData == null)
        {
            return "";
        }

        StringBuilder builder = new();

        if (saveData.BonusHP > 0)
        {
            AppendStat(builder, "HP", GameFormatUtils.ToIdleNumber(saveData.BonusHP));
        }

        if (saveData.BonusAtk > 0)
        {
            AppendStat(builder, "ATK", GameFormatUtils.ToIdleNumber(saveData.BonusAtk));
        }

        if (saveData.BonusDef > 0)
        {
            AppendStat(builder, "DEF", GameFormatUtils.ToIdleNumber(saveData.BonusDef));
        }

        if (saveData.BonusCriChance > 0)
        {
            AppendStat(builder, "CRI", GameFormatUtils.ToPercent((float)(saveData.BonusCriChance / 100f)));
        }

        return builder.ToString();
    }

    private void AppendStat(StringBuilder builder, string label, string value)
    {
        if (builder.Length > 0)
        {
            builder.AppendLine();
        }

        builder.Append($"{label} + {value}");
    }
}

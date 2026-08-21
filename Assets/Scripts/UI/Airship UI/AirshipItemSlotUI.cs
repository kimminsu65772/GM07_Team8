using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirshipItemSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;  
    [SerializeField] private TextMeshProUGUI descText;

    [Header("Equip Effect")]
    [SerializeField] private GameObject goldGlowObject;

    [Header("Lock Effect")]
    [SerializeField] private GameObject lockObject;

    // 대포
    public void SetCannonInfo(AirshipCannonData data)
    {
        if (data != null)
        {
            if (iconImage != null) iconImage.sprite = data.UIImage;
            if (nameText != null) nameText.text = data.DisplayName;
            if (descText != null) descText.text = $"타입: {data.CannonType}";
        }
    }
    // 기어
    public void SetGearInfo(AirshipGearData data)
    {
        if (data != null)
        {
            if (iconImage != null) iconImage.sprite = data.GearImage;
            if (nameText != null) nameText.text = data.DisplayName;
            if (descText != null) descText.text = $"타입: {data.GearType}";
        }
    }
    //장착
    public void SetEquippedState(bool isEquipped)
    {
        if (goldGlowObject != null)
        {
            goldGlowObject.SetActive(isEquipped);
        }
    }
    //잠금
    public void SetLockedState(bool isLocked)
    {
        if (lockObject != null)
        {
            lockObject.SetActive(isLocked);
        }
    }
}
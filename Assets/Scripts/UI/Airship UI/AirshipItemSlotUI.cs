using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

    [Header("Buttons")]
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unlockButton;

    [Header("Unlock Button Alpha")]
    [SerializeField] private float unlockAvailableAlpha = 1f;
    [SerializeField] private float unlockUnavailableAlpha = 0.4f;

    private Coroutine unlockFailCoroutine;
    public Button EquipButton
    {
        get { return equipButton; }
    }
    public Button UnlockButton
    {
        get { return unlockButton; }
    }
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
        if (unlockButton != null)
        {
            unlockButton.gameObject.SetActive(isLocked);
            if (isLocked)
            {
                unlockButton.interactable = true;
            }
        }
    }
    //해금 버튼 밝기
    public void SetUnlockAvailable(bool canUnlock)
    {
        if (unlockButton == null) return;

        Image buttonImage =  unlockButton.GetComponent<Image>();

        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = canUnlock ? unlockAvailableAlpha : unlockUnavailableAlpha;

            buttonImage.color = color;
        }
        unlockButton.interactable = true;
    }
}

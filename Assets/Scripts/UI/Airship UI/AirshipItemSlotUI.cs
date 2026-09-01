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
    [SerializeField] private TextMeshProUGUI effectDescText; //파츠 상세 설명

    [Header("Unlock Text References")] 
    [SerializeField] private TextMeshProUGUI conditionText; 
    [SerializeField] private TextMeshProUGUI costText;      
    [SerializeField] private GameObject costObject;

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

    public Button EquipButton => equipButton;
    public Button UnlockButton => unlockButton;

    // 대포
    public void SetCannonInfo(AirshipCannonData data)
    {
        if (data != null)
        {
            if (iconImage != null) iconImage.sprite = data.UIImage;
            if (nameText != null) nameText.text = data.DisplayName;
            if (descText != null) descText.text = $"타입: {data.CannonType}";

            // 대포 데이터(AirshipCannonData) 내부에 설명/효과 변수 추가시 여기에 연결하세요.
        }
    }
    // 기어
    public void SetGearInfo(AirshipGearData data)
    {
        if (data != null)
        {
            if (iconImage != null) iconImage.sprite = data.UIImage;
            if (nameText != null) nameText.text = data.DisplayName;
            if (descText != null) descText.text = $"타입: {data.GearType}";

            // 기어 데이터(AirshipGearData) 내부에 설명/효과 변수 추가시 여기에 연결하세요.
        }
    }
    public void SetUnlockInfo(string conditionDesc, string costTextString)
    {
        if (conditionText != null)
        {
            conditionText.text = conditionDesc; 
        }

        if (costText != null)
        {
            costText.text = $"x{costTextString}";    
        }

        if (costObject != null)
        {
            costObject.SetActive(true);
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

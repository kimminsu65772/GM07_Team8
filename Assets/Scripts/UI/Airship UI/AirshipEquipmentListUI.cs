using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AirshipEquipmentListUI : MonoBehaviour
{
    public enum ListType { CannonList, GearList }
    [SerializeField] private ListType listType;

    [Header("UI References")]
    [SerializeField] private Transform contentTransform;
    [SerializeField] private GameObject itemSlotPrefab;

    [Header("Unlock")]
    [SerializeField] private AirshipPartsUnlockCatalog unlockCatalog;

    [Header("Unlock 실패 안내")]
    [SerializeField] private GameObject unlockFailPanel;
    [SerializeField] private TextMeshProUGUI unlockFailText;
    [SerializeField] private float unlockFailDuration = 1.5f;

    private AirshipEquipmentController equipmentController;
    private Coroutine unlockFailCoroutine;

    private void OnEnable()
    {
        if (equipmentController == null)
            equipmentController = FindAnyObjectByType<AirshipEquipmentController>();

        if (unlockFailPanel != null)
            unlockFailPanel.SetActive(false);
        RefreshList();
    }
    public void RefreshList()
    {
        if (equipmentController == null) return;
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        if (listType == ListType.CannonList)
        {
            var cannons = GetPrivateFieldList<AirshipCannonData>(equipmentController, "cannonDatas");
            if (cannons == null) return;

            foreach (var data in cannons)
            {
                GameObject slot = Instantiate(itemSlotPrefab, contentTransform);

                AirshipItemSlotUI slotUI = slot.GetComponent<AirshipItemSlotUI>();
                if (slotUI != null) slotUI.SetCannonInfo(data);

                Button btn = slotUI.EquipButton;
                TextMeshProUGUI btnText = btn != null ? btn.GetComponentInChildren<TextMeshProUGUI>() : null;
                // 해금 여부
                bool isOwned = PlayerInfo.Instance.IsCannonOwned(data.CannonType);
                // 장착 여부
                bool isEquipped = (equipmentController.EquippedCannon == data);
                // 해금 가능 여부
                bool canUnlock = false;
                if (!isOwned && unlockCatalog != null)
                {
                    UnlockResult unlockResult = unlockCatalog.CheckCannonUnlock(data.CannonType);
                    canUnlock = unlockResult.CanUnlock;
                }
                if (slotUI != null)
                {
                    slotUI.SetLockedState(!isOwned);
                    slotUI.SetEquippedState(isEquipped);
                    if (!isOwned)
                    {
                        slotUI.SetUnlockAvailable(canUnlock);
                        Button unlockButton = slotUI.UnlockButton;
                        if (unlockButton != null)
                        {
                            unlockButton.onClick.RemoveAllListeners();
                            unlockButton.onClick.AddListener(() =>
                            {
                                OnClickUnlockCannonButton(data.CannonType);
                            });
                        }
                    }
                }
                if (btn != null)
                {
                    btn.interactable = isOwned;
                }
                if (btnText != null)
                {
                    btnText.text = isEquipped ? "해제" : "장착";
                }

                if (btn != null)
                {
                    btn.onClick.AddListener(() => {
                        if (isEquipped)
                        {
                            ToggleUnequipCannon();
                        }
                        else
                        {
                            OnClickEquipCannonButton(data.CannonType);
                        }
                        RefreshList(); // 클릭 후 버튼 텍스트와 상태 즉시 갱신
                    });
                }
            }
        }
        else if (listType == ListType.GearList)
        {
            var gears = GetPrivateFieldList<AirshipGearData>(equipmentController, "gearDatas");
            if (gears == null) return;

            foreach (var data in gears)
            {
                GameObject slot = Instantiate(itemSlotPrefab, contentTransform);
                AirshipItemSlotUI slotUI = slot.GetComponent<AirshipItemSlotUI>();
                if (slotUI != null) slotUI.SetGearInfo(data);

                Button btn = slotUI.EquipButton;
                TextMeshProUGUI btnText = btn != null ? btn.GetComponentInChildren<TextMeshProUGUI>() : null;
                // 해금 여부
                bool isOwned = PlayerInfo.Instance.IsGearOwned(data.GearType);
                // 장착 여부
                bool isEquipped = (equipmentController.EquippedGear == data);
                // 해금 가능 여부
                bool canUnlock = false;
                if (!isOwned && unlockCatalog != null)
                {
                    UnlockResult unlockResult = unlockCatalog.CheckGearUnlock(data.GearType);
                    canUnlock = unlockResult.CanUnlock;
                }
                if (slotUI != null)
                {
                    slotUI.SetLockedState(!isOwned);
                    slotUI.SetEquippedState(isEquipped);
                    if (!isOwned)
                    {
                        slotUI.SetUnlockAvailable(canUnlock);
                        Button unlockButton = slotUI.UnlockButton;
                        if (unlockButton != null)
                        {
                            unlockButton.onClick.RemoveAllListeners();
                            unlockButton.onClick.AddListener(() =>
                            {
                                OnClickUnlockGearButton(data.GearType);
                            });
                        }
                    }
                }
                if (btn != null)
                {
                    btn.interactable = isOwned;
                }
                if (btnText != null)
                {
                    btnText.text = isEquipped ? "해제" : "장착";
                }

                if (btn != null)
                {
                    btn.onClick.AddListener(() => 
                    {
                        if (isEquipped)
                        {
                            ToggleUnequipGear();
                        }
                        else
                        {
                            OnClickEquipGearButton(data.GearType);
                        }
                        RefreshList();
                    });
                }
            }
        }
    }
    public void OnClickEquipCannonButton(AirshipCannonType cannonType)
    {
        if (equipmentController != null)
        {
            equipmentController.EquipCannon(cannonType);
        }
    }
    public void OnClickEquipGearButton(AirshipGearType gearType)
    {
        if (equipmentController != null)
        {
            equipmentController.EquipGear(gearType);
        }
    }
    private void OnClickUnlockCannonButton(AirshipCannonType cannonType)
    {
        if (unlockCatalog == null) return;

        UnlockResult unlockResult = unlockCatalog.CheckCannonUnlock(cannonType);
        // 조건 부족
        if (!unlockResult.IsRequirementMet)
        {
            ShowUnlockFailPanel("해금 조건을 만족하지 않았습니다.");
            return;
        }
        // 재화 부족
        if (!unlockResult.HasEnoughCurrency)
        {
            ShowUnlockFailPanel("재화가 부족합니다.");
            return;
        }
        bool success = PlayerInfo.Instance.TrySpendCurrency(unlockResult.Cost.Type, unlockResult.Cost.Amount, SavePolicy.Soon);
        if (!success)
        {
            ShowUnlockFailPanel("재화가 부족합니다.");
            return;
        }
        PlayerInfo.Instance.SetOwnedCannonId(cannonType, SavePolicy.Soon);
        RefreshList();
    }
    private void OnClickUnlockGearButton(AirshipGearType gearType)
    {
        if (unlockCatalog == null) return;

        UnlockResult unlockResult = unlockCatalog.CheckGearUnlock(gearType);
        // 조건 부족
        if (!unlockResult.IsRequirementMet)
        {
            ShowUnlockFailPanel("해금 조건을 만족하지 않았습니다.");
            return;
        }
        // 재화 부족
        if (!unlockResult.HasEnoughCurrency)
        {
            ShowUnlockFailPanel("재화가 부족합니다.");
            return;
        }
        bool success = PlayerInfo.Instance.TrySpendCurrency(unlockResult.Cost.Type, unlockResult.Cost.Amount, SavePolicy.Soon);
        if (!success)
        {
            ShowUnlockFailPanel("재화가 부족합니다.");
            return;
        }
        PlayerInfo.Instance.SetOwnedGearId(gearType, SavePolicy.Soon);
        RefreshList();
    }
    private void ShowUnlockFailPanel(string message)
    {
        if (unlockFailPanel == null || unlockFailText == null) return;
        unlockFailText.text = message;
        unlockFailPanel.SetActive(true);

        if (unlockFailCoroutine != null)
        {
            StopCoroutine(unlockFailCoroutine);
        }
        unlockFailCoroutine = StartCoroutine(HideUnlockFailPanel());
    }
    private IEnumerator HideUnlockFailPanel()
    {
        yield return new WaitForSeconds(unlockFailDuration);

        if (unlockFailPanel != null)
        {
            unlockFailPanel.SetActive(false);
        }
        unlockFailCoroutine = null;
    }
    private void ToggleUnequipCannon()
    {
        if (equipmentController == null) return;

        MethodInfo method = equipmentController.GetType().GetMethod("UnequipCannon", BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(equipmentController, null);

            FieldInfo field = equipmentController.GetType().GetField("OnCannonChanged", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                System.Action<AirshipCannonData> del = field.GetValue(equipmentController) as System.Action<AirshipCannonData>;
                del?.Invoke(null);
            }
            equipmentController.EquipCannon(AirshipCannonType.Normal);
        }
    }
    private void ToggleUnequipGear()
    {
        if (equipmentController == null) return;

        MethodInfo method = equipmentController.GetType().GetMethod("UnequipGear", BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(equipmentController, null);

            FieldInfo field = equipmentController.GetType().GetField("OnGearChanged", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                System.Action<AirshipGearData> del = field.GetValue(equipmentController) as System.Action<AirshipGearData>;
                del?.Invoke(null);
            }
            equipmentController.EquipGear(AirshipGearType.Normal);
        }
    }
    private List<T> GetPrivateFieldList<T>(object target, string fieldName) where T : class
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            return field.GetValue(target) as List<T>;
        }
        return null;
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Reflection;
public class AirshipEquipmentListUI : MonoBehaviour
{
    public enum ListType { CannonList, GearList }
    [SerializeField] private ListType listType;

    [Header("UI References")]
    [SerializeField] private Transform contentTransform;
    [SerializeField] private GameObject itemSlotPrefab;

    [Header("Unlock")]
    [SerializeField] private AirshipPartsUnlockCatalog unlockCatalog;

    private AirshipEquipmentController equipmentController;

    private void OnEnable()
    {
        if (equipmentController == null)
        equipmentController = FindAnyObjectByType<AirshipEquipmentController>();

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
                                RefreshList();
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
                                RefreshList();
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
    private List<T> GetPrivateFieldList<T>(object target, string fieldName) where T : class
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            return field.GetValue(target) as List<T>;
        }
        return null;
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
        if (!unlockResult.CanUnlock) return;

        if (!PlayerInfo.Instance.TrySpendCurrency(unlockResult.Cost.Type, unlockResult.Cost.Amount, SavePolicy.Soon))
        {
            return;
        }

        PlayerInfo.Instance.SetOwnedCannonId(cannonType, SavePolicy.Soon);
    }
    private void OnClickUnlockGearButton(AirshipGearType gearType)
    {
        if (unlockCatalog == null) return;

        UnlockResult unlockResult = unlockCatalog.CheckGearUnlock(gearType);
        if (!unlockResult.CanUnlock) return;

        if (!PlayerInfo.Instance.TrySpendCurrency(unlockResult.Cost.Type, unlockResult.Cost.Amount, SavePolicy.Soon))
        {
            return;
        }

        PlayerInfo.Instance.SetOwnedGearId(gearType, SavePolicy.Soon);
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
}

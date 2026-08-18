using UnityEngine;
using UnityEngine.UI;
public class AirshipEquippedSlotUI : MonoBehaviour
{
    public enum EquipmentSlotType { Cannon, Gear }

    [SerializeField] private EquipmentSlotType slotType;
    [SerializeField] private Image iconImage; 

    private AirshipEquipmentController equipmentController;

    private void Start() 
    { 
        equipmentController = FindAnyObjectByType<AirshipEquipmentController>(); 
        if (equipmentController != null) 
        {
            if (slotType == EquipmentSlotType.Cannon) 
            { 
                equipmentController.OnCannonChanged += UpdateCannonSlot; 
                UpdateCannonSlot(equipmentController.EquippedCannon); 
            } 
            else
            { 
                equipmentController.OnGearChanged += UpdateGearSlot; 
                UpdateGearSlot(equipmentController.EquippedGear);
            } 
        } 
    } 
    private void OnDestroy() 
    { 
        if (equipmentController != null)
        { 
            equipmentController.OnCannonChanged -= UpdateCannonSlot; 
            equipmentController.OnGearChanged -= UpdateGearSlot; 
        } 
    } 
    private void UpdateCannonSlot(AirshipCannonData data) 
    { 
        if (slotType != EquipmentSlotType.Cannon) return;
        if (data != null && data.UIImage != null) 
        {
            iconImage.sprite = data.UIImage; 
            iconImage.gameObject.SetActive(true); 
        } 
        else 
        {
            iconImage.sprite = null; 
            iconImage.gameObject.SetActive(false); 
        }
    }
    private void UpdateGearSlot(AirshipGearData data) 
    {
        if (slotType != EquipmentSlotType.Gear) return;
        if (data != null && data.UIImage != null)
        { 
            iconImage.sprite = data.UIImage; 
            iconImage.gameObject.SetActive(true);
        }
        else
        { 
            iconImage.sprite = null; 
            iconImage.gameObject.SetActive(false); 
        }
    } 
}

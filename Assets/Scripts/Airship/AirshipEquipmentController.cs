using System;
using UnityEngine;

public class AirshipEquipmentController : MonoBehaviour
{
    [Header("현재 장착")]
    [SerializeField] private AirshipCannonData equippedCannon;
    [SerializeField] private AirshipGearData equippedGear;

    [Header("참조")]
    [SerializeField] private AirshipStatController statController;
    [SerializeField] private SpriteRenderer barrelRenderer;
    [SerializeField] private SpriteRenderer gearRenderer;

    public AirshipCannonData EquippedCannon => equippedCannon;
    public AirshipGearData EquippedGear => equippedGear;

    public event Action<AirshipCannonData> OnCannonChanged;
    public event Action<AirshipGearData> OnGearChanged;

    private void Awake()
    {
        if (statController == null)
            statController = GetComponent<AirshipStatController>();

        ApplyCannonVisual();
        ApplyGearVisual();
        ApplyGearBuff();
    }

    public void EquipCannon(AirshipCannonData cannon)
    {
        if (cannon == null)
            return;

        equippedCannon = cannon;
        ApplyCannonVisual();

        OnCannonChanged?.Invoke(equippedCannon);
    }

    public void EquipGear(AirshipGearData gear)
    {
        if (gear == null)
            return;

        RemoveEquippedGearBuff();

        equippedGear = gear;
        ApplyGearBuff();
        ApplyGearVisual();

        OnGearChanged?.Invoke(equippedGear);
    }

    public void UnequipGear()
    {
        if (equippedGear == null)
            return;

        RemoveEquippedGearBuff();

        equippedGear = null;
        ApplyGearVisual();

        OnGearChanged?.Invoke(null);
    }

    private void ApplyGearBuff()
    {
        if (statController == null || equippedGear == null)
            return;

        AirshipBuff buff = equippedGear.CreateBuff(this);
        statController.AddBuff(buff);
    }

    private void RemoveEquippedGearBuff()
    {
        if (statController == null)
            return;

        statController.RemoveBuffsByOwner(this);
    }

    private void ApplyCannonVisual()
    {
        if (barrelRenderer == null)
            return;

        barrelRenderer.sprite =
            equippedCannon == null
                ? null
                : equippedCannon.BarrelImage;
    }

    private void ApplyGearVisual()
    {
        if (gearRenderer == null)
            return;

        gearRenderer.sprite =
            equippedGear == null
                ? null
                : equippedGear.GearImage;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class AirshipEquipmentController : MonoBehaviour
{
    [Header("현재 장착")]
    [SerializeField] private AirshipCannonData equippedCannon;
    [SerializeField] private AirshipGearData equippedGear;

    [Header("장비 데이터")]
    [SerializeField] private List<AirshipCannonData> cannonDatas =
        new List<AirshipCannonData>();

    [SerializeField] private List<AirshipGearData> gearDatas =
        new List<AirshipGearData>();

    [Header("참조")]
    [SerializeField] private AirshipStatController statController;
    [SerializeField] private SpriteRenderer barrelRenderer;
    [SerializeField] private SpriteRenderer gearRenderer;

    private readonly Dictionary<AirshipCannonType, AirshipCannonData> cannonDataDictionary = new Dictionary<AirshipCannonType, AirshipCannonData>();
    private readonly Dictionary<AirshipGearType, AirshipGearData> gearDataDictionary = new Dictionary<AirshipGearType, AirshipGearData>();

    // 장비 슬롯별 고유 owner
    private readonly object cannonBuffOwner = new object();
    private readonly object gearBuffOwner = new object();

    public AirshipCannonData EquippedCannon => equippedCannon;
    public AirshipGearData EquippedGear => equippedGear;

    public event Action<AirshipCannonData> OnCannonChanged;
    public event Action<AirshipGearData> OnGearChanged;

    private void Awake()
    {
        if (statController == null)
            statController = GetComponent<AirshipStatController>();
    }
    public void Init()
    {
        CreateCannonDictionary();
        CreateGearDictionary();
        LoadState();
    }
    private void CreateCannonDictionary()
    {
        cannonDataDictionary.Clear();

        foreach (AirshipCannonData cannonData in cannonDatas)
        {
            if (cannonData == null)
                continue;

            if (cannonDataDictionary.ContainsKey(cannonData.CannonType))
            {

                continue;
            }

            cannonDataDictionary.Add(
                cannonData.CannonType,
                cannonData
            );
        }
    }
    private void CreateGearDictionary()
    {
        gearDataDictionary.Clear();

        foreach (AirshipGearData gearData in gearDatas)
        {
            if (gearData == null)
                continue;

            if (gearDataDictionary.ContainsKey(gearData.GearType))
            {

                continue;
            }

            gearDataDictionary.Add(
                gearData.GearType,
                gearData
            );
        }
    }
    private void LoadState()
    {
        PlayerInfo playerInfo = PlayerInfo.Instance;
        AirshipSaveData saveData = playerInfo.Airship;

        if (saveData == null)
        {
            return;
        }

        EquipCannon(saveData.EquippedCannonType);
        EquipGear(saveData.EquippedGearType);
    }
    public void EquipCannon(AirshipCannonType cannonType)
    {
        if (!cannonDataDictionary.TryGetValue(
                cannonType,
                out AirshipCannonData cannonData))
        {

            return;
        }

        UnequipCannon();

        equippedCannon = cannonData;

        ApplyCannonBuff();
        ApplyCannonVisual();

        OnCannonChanged?.Invoke(equippedCannon);
        PlayerInfo.Instance.SetEquippedCannonId(equippedCannon.CannonType);
    }
    public void EquipGear(AirshipGearType gearType)
    {
        if (!gearDataDictionary.TryGetValue(
                gearType,
                out AirshipGearData gearData))
        {

            return;
        }

        UnequipGear();

        equippedGear = gearData;

        ApplyGearBuff();
        ApplyGearVisual();

        OnGearChanged?.Invoke(equippedGear);
        PlayerInfo.Instance.SetEquippedGearId(equippedGear.GearType);
    }
    private void UnequipCannon()
    {
        if (statController != null)
            statController.RemoveBuffsByOwner(cannonBuffOwner);

        equippedCannon = null;
    }
    private void UnequipGear()
    {
        if (statController != null)
            statController.RemoveBuffsByOwner(gearBuffOwner);

        equippedGear = null;
    }
    private void ApplyCannonBuff()
    {
        if (statController == null || equippedCannon == null)
            return;

        AirshipBuff buff =
            equippedCannon.CreateBuff(cannonBuffOwner);

        statController.AddBuff(buff);
    }
    private void ApplyGearBuff()
    {
        if (statController == null || equippedGear == null)
            return;

        AirshipBuff buff =
            equippedGear.CreateBuff(gearBuffOwner);

        statController.AddBuff(buff);
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
    [ContextMenu("Freeze")]
    public void TestFreeze()
    {
        EquipCannon(AirshipCannonType.Freeze);
    }
    [ContextMenu("Rapid")]
    public void TestRapid()
    {
        EquipCannon(AirshipCannonType.Rapid);
    }
    [ContextMenu("Heal")]
    public void TestHeal()
    {
        EquipCannon(AirshipCannonType.Heal);
    }
}
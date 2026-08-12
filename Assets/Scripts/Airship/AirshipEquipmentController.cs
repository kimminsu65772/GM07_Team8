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

    private readonly Dictionary<AirshipCannonType, AirshipCannonData>
        cannonDataDictionary =
        new Dictionary<AirshipCannonType, AirshipCannonData>();

    private readonly Dictionary<AirshipGearType, AirshipGearData>
        gearDataDictionary =
        new Dictionary<AirshipGearType, AirshipGearData>();

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
                Debug.LogError(
                    $"중복된 대포 타입이 등록되어 있습니다. " +
                    $"Type: {cannonData.CannonType}"
                );

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
                Debug.LogError(
                    $"중복된 기어 타입이 등록되어 있습니다. " +
                    $"Type: {gearData.GearType}"
                );

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
            Debug.LogError("Airship save data is missing.");
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
            Debug.LogWarning(
                $"장착할 대포를 찾을 수 없습니다. Type: {cannonType}"
            );

            return;
        }

        equippedCannon = cannonData;
        ApplyCannonVisual();

        OnCannonChanged?.Invoke(equippedCannon);
    }

    public void EquipGear(AirshipGearType gearType)
    {
        if (!gearDataDictionary.TryGetValue(
                gearType,
                out AirshipGearData gearData))
        {
            Debug.LogWarning(
                $"장착할 기어를 찾을 수 없습니다. Type: {gearType}"
            );

            return;
        }

        UnequipGear();

        equippedGear = gearData;
        ApplyGearBuff();
        ApplyGearVisual();

        OnGearChanged?.Invoke(equippedGear);
    }

    private void UnequipGear()
    {
        if (equippedGear == null)
            return;

        if (statController != null)
        {
            statController.RemoveBuffsByOwner(this);
        }

        equippedGear = null;
    }

    private void ApplyGearBuff()
    {
        if (statController == null || equippedGear == null)
            return;

        AirshipBuff buff = equippedGear.CreateBuff(this);
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
}
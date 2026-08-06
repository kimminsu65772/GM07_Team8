using System;
using UnityEngine;

/// <summary>
/// 스탯 업그레이드 로직을 담당. <br/>
/// 업그레이드 가능 여부 판단과 ui연결을 여기서 할듯.
/// </summary>
public class AirshipUpgradeManager : MonoBehaviour
{
    private readonly AirshipUpgradeState upgradeState = new AirshipUpgradeState();

    public AirshipUpgradeState UpgradeState => upgradeState;

    //UI, 저장 요청, 사운드/이펙트, 스탯컨트롤러에 스탯 재계산을 AirshipController에서 등록.
    public event Action<AirshipUpgradeState> OnUpgradeChanged;

    public void Init(AirshipSaveData saveData)
    {
        LoadFromSaveData(saveData);
    }

    public bool CanUpgrade(AirshipStatType statType)
    {
        return HasUpgradeCost(statType);
    }

    public bool TryUpgrade(AirshipStatType statType)
    {
        if (!CanUpgrade(statType))
        {
            return false;
        }

        if (!SpendUpgradeCost(statType))
        {
            return false;
        }

        upgradeState.IncreaseStatLevel(statType);
        OnUpgradeChanged?.Invoke(upgradeState);
        return true;
    }

    private bool HasUpgradeCost(AirshipStatType statType)
    {
        // TODO: 재화 시스템 연결 후 비용 확인.
        return true;
    }

    private bool SpendUpgradeCost(AirshipStatType statType)
    {
        // TODO: 재화 시스템 연결 후 비용 차감.
        return true;
    }
    
    public void LoadFromSaveData(AirshipSaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        upgradeState.SetLevels(
            saveData.AttackLevel,
            saveData.DefenseLevel,
            saveData.MaxHealthLevel,
            saveData.CriticalLevel
        );

        OnUpgradeChanged?.Invoke(upgradeState);
    }

    public void ApplyToSaveData(AirshipSaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        saveData.AttackLevel = upgradeState.AttackLevel;
        saveData.DefenseLevel = upgradeState.DefenseLevel;
        saveData.MaxHealthLevel = upgradeState.MaxHealthLevel;
        saveData.CriticalLevel = upgradeState.CriticalLevel;
    }
}
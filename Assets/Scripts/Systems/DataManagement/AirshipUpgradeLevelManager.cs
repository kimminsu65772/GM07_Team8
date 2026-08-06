using UnityEngine;
using System;

public class AirshipUpgradeLevelManager : MonoBehaviour
{
    public static AirshipUpgradeLevelManager Instance { get; private set; }

    private AirshipSaveData airshipSaveData;
    private bool isInitialized;

    public bool IsInitialized => isInitialized;

    public void Initialize(AirshipSaveData saveData)
    {
        if (saveData == null)
        {
            throw new ArgumentNullException(nameof(saveData), "에어쉽 저장 데이터가 비어 있습니다.");
        }
        airshipSaveData = saveData;
        isInitialized = true;
    }

    public void SetAirshipLevelData(AirshipUpgradeState upgradeState)
    {
        if (upgradeState == null)
        {
            throw new ArgumentNullException(nameof(upgradeState), "업그레이드 상태가 비어 있습니다.");
        }
        
        airshipSaveData.AttackLevel = upgradeState.AttackLevel;
        airshipSaveData.DefenseLevel = upgradeState.DefenseLevel;
        airshipSaveData.MaxHealthLevel = upgradeState.MaxHealthLevel;
        airshipSaveData.CriticalLevel = upgradeState.CriticalLevel;
    }

    public AirshipSaveData GetAirshipLevelData()
    {
        if (!isInitialized)
        {
            throw new InvalidOperationException("AirshipUpgradeLevelManager가 초기화되지 않았습니다.");
        }
        return airshipSaveData;
    }
}

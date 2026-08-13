using System;
using UnityEngine;

public class AirshipUpgradeController : MonoBehaviour
{
    // 아직 재화를 안정하고, 레벨당 요구량 테이블도 없어서 일단 임시로 해둠.
    [SerializeField] private CurrencyType upgradeCurrency = CurrencyType.Gold;
    [SerializeField, Min(0)] private int upgradeCost = 100;
    [SerializeField] private AirshipStatTable statTable;

    // 일단 스탯이 0인 상태를 생성
    // LoadState에서 세이브 값으로 갱신된다.
    private readonly AirshipUpgradeState upgradeState =
        new AirshipUpgradeState();

    // 세이브 데이터가 런타임 상태에 적용되었는지 확인.
    private bool isInitialized;

    public AirshipUpgradeState UpgradeState => upgradeState;
    public CurrencyType UpgradeCurrency => upgradeCurrency;
    public int UpgradeCost => upgradeCost;

    public event Action<AirshipUpgradeState> OnUpgradeChanged;

    public void Init()
    {
        LoadState();
    }

    // 세이브 데이터 기준으로 로드.
    // 아마 init할때만 쓸거같지만, 뭔가 씬을 옮길때 쓸수도.
    public void LoadState()
    {
        AirshipSaveData saveData = PlayerInfo.Instance.Airship;

        if (saveData == null)
        {
            Debug.LogError("Airship save data is missing.");
            return;
        }

        upgradeState.SetLevels(
            saveData.AttackLevel,
            saveData.DefenseLevel,
            saveData.MaxHealthLevel,
            saveData.CriticalLevel
        );

        isInitialized = true;
        NotifyUpgradeChanged();
    }

    public bool TryUpgrade(AirshipStatType statType)
    {
        if (!isInitialized)
        {
            return false;
        }

        int currentLevel = upgradeState.GetLevel(statType);
        // -1인 스탯, 이속 공속은 업그레이드 대상이 아니므로 혹시 모를 방지는 해둠.
        if (currentLevel < 0)
        {
            return false;
        }

        // 이미 최대 레벨이면 막음
        if (currentLevel >= statTable.GetMaxLevel(statType))
        {
            return false;
        }

        if (!TrySpendUpgradeCost())
        {
            Debug.Log("재화 부족");
            return false;
        }

        upgradeState.IncreaseStatLevel(statType);
        SaveState();
        NotifyUpgradeChanged();

        return true;
    }

    // TODO 코스트를 어떻게 할지 정하면 데이터 테이블과 세부로직 정하기.
    private bool TrySpendUpgradeCost()
    {
        return PlayerInfo.Instance.TrySpendCurrency(
            upgradeCurrency,
            upgradeCost
        );
    }

    private void SaveState()
    {
        PlayerInfo.Instance.SetAirshipUpgradeState(
            upgradeState
        );
    }

    private void NotifyUpgradeChanged()
    {
        OnUpgradeChanged?.Invoke(upgradeState);
    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    

    #region UI 관련 함수들

    public bool IsMaxLevel(AirshipStatType statType)
    {
        return upgradeState.GetLevel(statType) >= statTable.GetMaxLevel(statType);
    }

    public int GetCurrentLevel(AirshipStatType statType)
    {
        return upgradeState.GetLevel(statType);
    }

    public float GetCurrentStat(AirshipStatType statType)
    {
        return statTable.GetStatValue(statType, GetCurrentLevel(statType));
    }

    // ui 원초적으론 ui에서 ismaxlevel이 true일땐 이게 호출되면 안되게 설계해야함.
    // 다만 혹시 모를 상황을 대비해 방지해둠.
    public int GetNextLevel(AirshipStatType statType)
    {
        if (IsMaxLevel(statType))
            return -1;
        return GetCurrentLevel(statType) + 1;
    }

    public float GetNextStat(AirshipStatType statType)
    {
        int nextLevel = GetNextLevel(statType);

        if (nextLevel < 0)
        {
            return -1f;
        }

        return statTable.GetStatValue(statType, nextLevel);
    }

    // TODO 코스트를 어떻게 할지 정하면 데이터 테이블과 세부로직 정하기.
    public int GetCost(AirshipStatType statType)
    {
        return upgradeCost;
    }

    #endregion
}
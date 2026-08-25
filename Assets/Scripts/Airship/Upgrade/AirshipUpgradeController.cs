using System;
using UnityEngine;

public class AirshipUpgradeController : MonoBehaviour
{
    // 기존에는 재화와 레벨당 요구량이 정해지지 않아 임시 비용을 사용했음.
    // 현재는 AirshipStatTable에서 스탯별, 레벨별 비용을 계산한다.
    [SerializeField] private CurrencyType upgradeCurrency = CurrencyType.Gold;
    [SerializeField] private AirshipStatTable statTable;

    // 성장 공식의 시작점인 기본 레벨 1 상태를 생성.
    // LoadState에서 세이브 값으로 갱신된다.
    private readonly AirshipUpgradeState upgradeState =
        new AirshipUpgradeState();

    // 세이브 데이터가 런타임 상태에 적용되었는지 확인.
    private bool isInitialized;

    public AirshipUpgradeState UpgradeState => upgradeState;
    public CurrencyType UpgradeCurrency => upgradeCurrency;

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
            saveData.RecoveryLevel,
            saveData.MaxHealthLevel,
            saveData.CriticalLevel
        );

        isInitialized = true;
        NotifyUpgradeChanged();
    }

    public bool TryUpgrade(AirshipStatType statType)
    {
        if (!isInitialized || statTable == null)
        {
            return false;
        }

        int currentLevel = upgradeState.GetLevel(statType);

        // -1인 스탯, 이속 공속은 업그레이드 대상이 아니므로 혹시 모를 방지는 해둠.
        if (currentLevel < 1)
        {
            return false;
        }

        // 이미 최대 레벨이면 막음
        if (currentLevel >= statTable.GetMaxLevel(statType))
        {
            return false;
        }

        if (!TrySpendUpgradeCost(statType))
        {
            Debug.Log("재화 부족");
            return false;
        }

        upgradeState.IncreaseStatLevel(statType);
        SaveState();
        NotifyUpgradeChanged();

        return true;
    }
    
    // AirshipStatTable에서 현재 레벨 기준 비용.
    private bool TrySpendUpgradeCost(AirshipStatType statType)
    {
        return PlayerInfo.Instance.TrySpendCurrency(
            upgradeCurrency,
            GetCost(statType)
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
        if (statTable == null)
        {
            return false;
        }

        return upgradeState.GetLevel(statType) >=
               statTable.GetMaxLevel(statType);
    }

    public int GetCurrentLevel(AirshipStatType statType)
    {
        return upgradeState.GetLevel(statType);
    }

    public double GetCurrentStat(AirshipStatType statType)
    {
        if (statTable == null)
        {
            return 0d;
        }

        return statTable.GetStatValue(
            statType,
            GetCurrentLevel(statType)
        );
    }

    // ui 원초적으론 ui에서 ismaxlevel이 true일땐 이게 호출되면 안되게 설계해야함.
    // 다만 혹시 모를 상황을 대비해 방지해둠.
    public int GetNextLevel(AirshipStatType statType)
    {
        if (statTable == null)
        {
            return -1;
        }

        int currentLevel = GetCurrentLevel(statType);

        if (currentLevel < 1 ||
            currentLevel >= statTable.GetMaxLevel(statType))
        {
            return -1;
        }

        return currentLevel + 1;
    }

    public double GetNextStat(AirshipStatType statType)
    {
        int nextLevel = GetNextLevel(statType);

        if (nextLevel < 0 || statTable == null)
        {
            return -1d;
        }

        return statTable.GetStatValue(
            statType,
            nextLevel
        );
    }

    // 비용은 현재 AirshipStatTable에서 계산되며, 아직 int.
    public int GetCost(AirshipStatType statType)
    {
        if (statTable == null)
        {
            return 0;
        }

        int currentLevel = GetCurrentLevel(statType);

        if (currentLevel < 1 ||
            currentLevel >= statTable.GetMaxLevel(statType))
        {
            return 0;
        }

        return statTable.GetUpgradeCost(
            statType,
            currentLevel
        );
    }

    #endregion
}
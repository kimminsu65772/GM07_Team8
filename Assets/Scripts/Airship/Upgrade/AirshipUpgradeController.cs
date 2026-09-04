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

    // upgradeLevelCount만큼 업그레이드한다.
    // 예: 1 = +1, 10 = +10, 100 = +100
    public bool TryUpgrade(
        AirshipStatType statType,
        int upgradeLevelCount)
    {
        if (!isInitialized ||
            statTable == null ||
            PlayerInfo.Instance == null ||
            upgradeLevelCount <= 0)
        {
            return false;
        }

        int currentLevel =
            upgradeState.GetLevel(statType);

        // -1인 스탯, 이속 공속은 업그레이드 대상이 아니므로 혹시 모를 방지는 해둠.
        if (currentLevel < 1)
        {
            return false;
        }

        // 이미 최대 레벨이면 막음
        if (IsMaxLevel(statType))
        {
            return false;
        }

        // 최대 레벨을 초과하지 않는 최종 레벨 계산
        int targetLevel =
            GetTargetLevel(statType, upgradeLevelCount);

        if (targetLevel <= currentLevel)
        {
            return false;
        }

        // 현재 레벨부터 최종 레벨까지 필요한 총 비용 계산
        long totalCost =
            GetUpgradeCost(statType, upgradeLevelCount);

        if (!PlayerInfo.Instance.TrySpendCurrency(
                upgradeCurrency,
                totalCost))
        {
            return false;
        }

        // 계산된 최종 레벨을 한 번에 반영
        upgradeState.SetLevel(
            statType,
            targetLevel
        );

        SaveState();
        NotifyUpgradeChanged();

        return true;
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
               GetMaxLevel(statType);
    }

    public int GetMaxLevel(AirshipStatType statType)
    {
        if (statTable == null)
        {
            return 0;
        }

        return statTable.GetMaxLevel(statType);
    }

    public int GetCurrentLevel(AirshipStatType statType)
    {
        return upgradeState.GetLevel(statType);
    }

    // 요청한 업그레이드 수를 적용했을 때의 최종 레벨.
    // 최대 레벨을 초과하지 않는다.
    public int GetTargetLevel(
        AirshipStatType statType,
        int upgradeLevelCount)
    {
        if (statTable == null ||
            upgradeLevelCount <= 0)
        {
            return -1;
        }

        int currentLevel =
            GetCurrentLevel(statType);

        if (currentLevel < 1)
        {
            return -1;
        }

        int maxLevel =
            GetMaxLevel(statType);

        if (maxLevel < 1)
        {
            return -1;
        }

        if (currentLevel >= maxLevel)
        {
            return maxLevel;
        }

        // int 오버플로 방지를 위해 long으로 먼저 계산
        long requestedTargetLevel =
            (long)currentLevel +
            upgradeLevelCount;

        return requestedTargetLevel >= maxLevel
            ? maxLevel
            : (int)requestedTargetLevel;
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

    // 요청한 업그레이드 수를 적용했을 때의 스탯.
    // 최대 레벨을 초과하면 최대 레벨의 스탯을 반환한다.
    public double GetTargetStat(
        AirshipStatType statType,
        int upgradeLevelCount)
    {
        if (statTable == null)
        {
            return -1d;
        }

        int targetLevel =
            GetTargetLevel(
                statType,
                upgradeLevelCount
            );

        if (targetLevel < 1)
        {
            return -1d;
        }

        return statTable.GetStatValue(
            statType,
            targetLevel
        );
    }

    // 현재 레벨에서 요청한 레벨 수만큼 올릴 때 필요한 총 비용.
    public long GetUpgradeCost(
        AirshipStatType statType,
        int upgradeLevelCount)
    {
        if (statTable == null ||
            upgradeLevelCount <= 0)
        {
            return 0L;
        }

        int currentLevel =
            GetCurrentLevel(statType);

        int targetLevel =
            GetTargetLevel(
                statType,
                upgradeLevelCount
            );

        if (currentLevel < 1 ||
            targetLevel <= currentLevel)
        {
            return 0L;
        }

        return statTable.GetTotalUpgradeCost(
            statType,
            currentLevel,
            targetLevel
        );
    }
    
    // 요청한 업그레이드 비용을 지불할 수 있는지 확인한다.
    public bool CanAffordUpgrade(
        AirshipStatType statType,
        int upgradeLevelCount)
    {
        if (!isInitialized ||
            statTable == null ||
            PlayerInfo.Instance == null ||
            upgradeLevelCount <= 0)
        {
            return false;
        }

        int currentLevel =
            GetCurrentLevel(statType);

        int targetLevel =
            GetTargetLevel(
                statType,
                upgradeLevelCount
            );

        if (currentLevel < 1 ||
            targetLevel <= currentLevel)
        {
            return false;
        }

        WalletSaveData wallet =
            PlayerInfo.Instance.Wallet;

        if (wallet == null ||
            wallet.Currencies == null)
        {
            return false;
        }

        if (!wallet.Currencies.TryGetValue(
                upgradeCurrency,
                out CurrencySaveData currency))
        {
            return false;
        }

        long requiredCost =
            GetUpgradeCost(
                statType,
                upgradeLevelCount
            );

        return currency.Amount >= requiredCost;
    }

    #endregion
}
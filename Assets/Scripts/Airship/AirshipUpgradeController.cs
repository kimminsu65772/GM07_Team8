using System;
using UnityEngine;

public class AirshipUpgradeController : MonoBehaviour
{
    // 아직 재화를 안정하고, 레벨당 요구량 테이블도 없어서 일단 임시로 해둠.
    [SerializeField] private CurrencyType upgradeCurrency = CurrencyType.Gold;
    [SerializeField, Min(0)] private int upgradeCost = 100;

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

        // -1인 스탯, 이속 공속은 업그레이드 대상이 아니므로 혹시 모를 방지는 해둠.
        if (upgradeState.GetLevel(statType) < 0)
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

    // ui쪽에서 enum을 기반으로 레벨을 가져갈때를 상정함.
    public int GetLevel(AirshipStatType statType)
    {
        return upgradeState.GetLevel(statType);
    }
}
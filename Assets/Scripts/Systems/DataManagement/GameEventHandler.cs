using UnityEngine;

public class GameEventHandler : MonoBehaviour
{
    [SerializeField] private AirshipUpgradeManager airshipUpgradeManager;
     
    // 초기화되기 전에는 이벤트를 처리하지 않도록 막는다.
    private bool isInitialized;

    // 이벤트 핸들러는 유일성을 보장 받아 중복으로 이벤트를 처리하지 않고 씬 전환 시에도 유지되어야 한다.
    // 따라서 싱글톤 패턴을 적용한다.

    private void OnEnable()
    {
        if (airshipUpgradeManager != null)
        {
            airshipUpgradeManager.OnUpgradeChanged -= HandleAirshipUpgradeChanged;
            airshipUpgradeManager.OnUpgradeChanged += HandleAirshipUpgradeChanged;
        }
    }

    private void OnDisable()
    {
        if (airshipUpgradeManager != null)
        {
            airshipUpgradeManager.OnUpgradeChanged -= HandleAirshipUpgradeChanged;
        }
    }

    public void Initialize(AirshipUpgradeManager airshipUpgradeManager)
    {
        if (airshipUpgradeManager == null)
        {
            Debug.LogError("AirshipUpgradeManager가 할당되지 않았습니다.");
            return;
        }
        this.airshipUpgradeManager = airshipUpgradeManager;
        isInitialized = true;
    }

    public void HandleMonsterKilled(CurrencyReward[] rewards)
    {
        if (!CanHandleEvent())
        {
            return;
        }

        GrantRewards(rewards);
        SaveScheduler.Instance.RequestSave(SavePolicy.Deferred);
    }

    public void HandleStageCleared(int clearedStage, CurrencyReward[] clearRewards)
    {
        if (!CanHandleEvent())
        {
            return;
        }

        if (StageProgressManager.Instance.TryUpdateMaxClearedStage(clearedStage))
        {
            GrantRewards(clearRewards);

            // 현재는 다음 스테이지로 바로 이동시킨다.
            // 갱신하려는 값이 마지막 스테이지를 넘어가는지 확인하는 방어코드 필요.
            StageProgressManager.Instance.SetCurrentStage(clearedStage + 1);
            SaveScheduler.Instance.RequestSave(SavePolicy.Soon);
        }
    }

    public void HandleLogBackOn(CurrencyReward[] offlineRewards)
    {
        if (!CanHandleEvent())
        {
            return;
        }

        GrantRewards(offlineRewards);
        SaveScheduler.Instance.RequestSave(SavePolicy.Immediate);
    }

    public void HandleStageChanged(int newStage)
    {
        if (!CanHandleEvent())
        {
            return;
        }

        StageProgressManager.Instance.SetCurrentStage(newStage);
        SaveScheduler.Instance.RequestSave(SavePolicy.Soon);
    }

    public bool HandlePremiumCurrencySpent(int amount)
    {
        if (!CanHandleEvent())
        {
            return false;
        }

        if (!WalletManager.Instance.TrySpend(CurrencyType.Gems, amount))
        {
            return false;
        }

        SaveScheduler.Instance.RequestSave(SavePolicy.Immediate);
        return true;
    }

    public void HandleAirshipUpgradeChanged(AirshipUpgradeState upgradeState)
    {
        if (!CanHandleEvent())
        {
            return;
        }

        AirshipUpgradeLevelManager.Instance.SetAirshipLevelData(upgradeState);
    }

    private bool CanHandleEvent()
    {
        return isInitialized;
    }

    private void GrantRewards(CurrencyReward[] rewards)
    {
        if (rewards == null || rewards.Length == 0)
        {
            return;
        }

        for (int i = 0; i < rewards.Length; i++)
        {
            CurrencyReward reward = rewards[i];
            WalletManager.Instance.TryAdd(reward.Type, reward.Amount);
        }
    }
}

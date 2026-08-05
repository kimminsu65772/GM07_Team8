using UnityEngine;

public class GameEventHandler : MonoBehaviour
{
    public static GameEventHandler Instance { get; private set; }

    private RuntimeDataContext dataContext;
    private SaveScheduler saveScheduler;

    // 초기화되기 전에는 이벤트를 처리하지 않도록 막는다.
    private bool isInitialized;

    // 이벤트 핸들러는 유일성을 보장 받아 중복으로 이벤트를 처리하지 않고 씬 전환 시에도 유지되어야 한다.
    // 따라서 싱글톤 패턴을 적용한다.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Initialize(RuntimeDataContext dataContext, SaveScheduler saveScheduler)
    {
        if (dataContext == null)
        {
            throw new System.ArgumentNullException(nameof(dataContext), "RuntimeDataContext가 초기화되지 않았습니다.");
        }

        if (saveScheduler == null)
        {
            throw new System.ArgumentNullException(nameof(saveScheduler), "SaveScheduler가 초기화되지 않았습니다.");
        }

        this.dataContext = dataContext;
        this.saveScheduler = saveScheduler;
        isInitialized = true;
    }

    public void HandleMonsterKilled(CurrencyReward[] rewards)
    {
        if (!CanHandleEvent())
        {
            return;
        }

        GrantRewards(rewards);
        saveScheduler.RequestSave(SavePolicy.Deferred);
    }

    public void HandleStageCleared(int clearedStage, CurrencyReward[] clearRewards)
    {
        if (!CanHandleEvent())
        {
            return;
        }

        if (dataContext.Stage.TryUpdateMaxClearedStage(clearedStage))
        {
            GrantRewards(clearRewards);

            // 현재는 다음 스테이지로 바로 이동시킨다.
            // 갱신하려는 값이 마지막 스테이지를 넘어가는지 확인하는 방어코드 필요.
            dataContext.Stage.SetCurrentStage(clearedStage + 1);
            saveScheduler.RequestSave(SavePolicy.Soon);
        }
    }

    public void HandleLogBackOn(CurrencyReward[] offlineRewards)
    {
        if (!CanHandleEvent())
        {
            return;
        }

        GrantRewards(offlineRewards);
        saveScheduler.RequestSave(SavePolicy.Immediate);
    }

    public void HandleStageChanged(int newStage)
    {
        if (!CanHandleEvent())
        {
            return;
        }

        dataContext.Stage.SetCurrentStage(newStage);
        saveScheduler.RequestSave(SavePolicy.Soon);
    }

    public bool HandleShipUpgrade(CurrencyCost cost)
    {
        if (!CanHandleEvent())
        {
            return false;
        }

        bool success = dataContext.Wallet.TrySpend(cost.Type, cost.Amount);
        if (success)
        {
            // 실제 업그레이드 로직은 업그레이드 담당 시스템이 확정된 뒤 연결한다.
            saveScheduler.RequestSave(SavePolicy.Deferred);
        }

        return success;
    }

    public bool HandlePremiumCurrencySpent(int amount)
    {
        if (!CanHandleEvent())
        {
            return false;
        }

        if (!dataContext.Wallet.TrySpend(CurrencyType.Gems, amount))
        {
            return false;
        }

        saveScheduler.RequestSave(SavePolicy.Immediate);
        return true;
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
            dataContext.Wallet.TryAdd(reward.Type, reward.Amount);
        }
    }
}

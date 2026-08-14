using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private HeroCatalog heroCatalog;

    private static PlayerInfo instance;

    public static PlayerInfo Instance => instance;

    public PlayerSaveData SaveData { get; private set; }
    public bool IsInitialized { get; private set; }

    public event Action<CurrencyType> OnCurrencyChanged;

    // UI에서 캐싱한 Hero 목록 상태를 갱신할 수 있도록 Hero 소유 여부 변경 시 이벤트 발생
    public event Action<string, bool> OnHeroOwnedChanged;

    public event Action<int> OnCurrentStageChanged;

    private SaveDataWriter saveDataWriter;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    private void Start()
    {
        OfflineRewardProvider.ProvideOfflineReward();
    }

    public void Initialize()
    {
        if (IsInitialized) return;

        if (heroCatalog == null)
        {
            Debug.LogError("PlayerInfo: HeroCatalog가 할당되지 않았습니다.");
            return;
        }

        string saveDataPath = Path.Combine(Application.persistentDataPath, "PlayerSaveData.json");

        saveDataWriter = new SaveDataWriter(saveDataPath);
        SaveDataLoader saveDataLoader = new SaveDataLoader(saveDataPath);

        if (saveDataLoader.Exists())
        {
            SaveData = saveDataLoader.Load();
        }
        else
        {
            SaveData = SaveDataFactory.CreateNewData(heroCatalog);
            saveDataWriter.ForceSave(SaveData);
        }

        Debug.Log($"{saveDataPath}");

        SaveScheduler.Instance.Initialize(SaveData, saveDataWriter);

        bool isAddedNewHero = false;

        foreach (HeroEntry entry in heroCatalog.InGameHeroEntries)
        {
            if (!SaveData.Heroes.ContainsKey(entry.HeroName))
            {
                SaveData.Heroes[entry.HeroName] = new HeroSaveData
                {
                    IsOwned = entry.IsDefaultOwned,
                    Level = entry.DefaultLevel
                };

                isAddedNewHero = true;
            }
        }

        if (isAddedNewHero)
        {
            saveDataWriter.ForceSave(SaveData);
        }

        IsInitialized = true;
    }

    // 세이브 데이터 조회를 위한 프로퍼티
    public PlayerProfileSaveData Profile => SaveData.Profile;
    public AirshipSaveData Airship => SaveData.Airship;
    public StageProgressSaveData StageProgress => SaveData.StageProgress;
    public WalletSaveData Wallet => SaveData.Wallet;

    public int CurrentStage => SaveData.StageProgress.CurrentStage;
    public int MaxClearedStage => SaveData.StageProgress.MaxClearedStage;
    public Dictionary<string, HeroSaveData> Heroes => SaveData.Heroes;

    public IReadOnlyList<HeroEntry> HeroEntries => heroCatalog.InGameHeroEntries;
    public HeroFormationSaveData HeroFormation => SaveData.HeroFormation;
    /// <summary>
    /// 재화 관련 수정 메서드
    /// </summary>
    /// <param name="type">변경할 재화 타입</param>
    /// <param name="amount">변경할 재화량 (음수는 불가)</param>
    /// <param name="savePolicy">
    /// SavePolicy.Deferred: 저장 요청 X, 다음 자동 저장 시점에 저장 
    /// SavePolicy.Soon: 설정된 시간 후에 저장 요청 (기본으로는 5초로 잡고 있음)
    /// SavePolicy.Immediate: 즉시 저장 요청
    /// </param>
    public void AddCurrency(CurrencyType type, int amount, SavePolicy savePolicy = SavePolicy.Deferred)
    {
        if (!CheckInitialized()) return;

        if (amount < 0)
        {
            Debug.LogError("AddCurrency: amount는 음수일 수 없습니다.");
            return;
        }


        // 실수로 타입을 정의하지 않은 경우를 대비하여, Wallet에 해당 타입이 없으면 초기화하여 추가
        if (!Wallet.Currencies.TryGetValue(type, out CurrencySaveData currency))
        {
            currency = new CurrencySaveData { Amount = 0 };
            Wallet.Currencies[type] = currency;
        }

        currency.Amount += amount;
        OnCurrencyChanged?.Invoke(type);

        RequestSave(savePolicy);
    }

    public bool TrySpendCurrency(CurrencyType type, int amount, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized()) return false;

        if (amount < 0)
        {
            Debug.LogError("TrySpendCurrency: amount는 음수일 수 없습니다.");
            return false;
        }
        if (!Wallet.Currencies.TryGetValue(type, out CurrencySaveData currency))
        {
            Debug.LogError($"TrySpendCurrency: 정의되지 않은 타입의 재화입니다. Type: {type}");
            return false;
        }
        if (currency.Amount < amount)
        {
            Debug.LogWarning($"TrySpendCurrency: 재화가 부족합니다. Type: {type}, Required: {amount}, Available: {currency.Amount}");
            return false;
        }
        currency.Amount -= amount;
        OnCurrencyChanged?.Invoke(type);
        RequestSave(savePolicy);
        return true;
    }

    /// <summary>
    /// 스테이지 진행 상황 업데이트 메서드
    /// </summary>
    /// <param name="stage">갱신하고자하는 스테이지 번호</param>
    /// <param name="savePolicy">저장 정책. 기본 설정은 Soon으로 스테이지 설정 변경 시 약 5초 후에 저장됨.</param>

    public bool TrySetCurrentStage(int stage, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized()) return false;
        if (stage < 1)
        {
            Debug.LogError("SetCurrentStage: stage는 1 이상이어야 합니다.");
            return false;
        }
        if (stage > SaveData.StageProgress.MaxClearedStage + 1)
        {
            Debug.LogWarning("SetCurrentStage: 현재 도전이 불가능한 스테이지를 선택하려고 합니다.");
            return false;
        }
        if (stage == SaveData.StageProgress.CurrentStage)
        {
            return true; // 이미 현재 스테이지와 동일한 값이면 저장 요청 없이 true 반환
        }

        StageProgress.CurrentStage = stage;
        OnCurrentStageChanged?.Invoke(stage);
        RequestSave(savePolicy);

        return true;
    }

    public bool TryUpdateMaxClearedStage(int stage, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized()) return false;
        if (stage < 1)
        {
            Debug.LogError("TryUpdateMaxClearedStage: stage는 1 이상이어야 합니다.");
            return false;
        }
        if (stage <= SaveData.StageProgress.MaxClearedStage)
        {
            Debug.LogWarning("TryUpdateMaxClearedStage: 현재 최대 클리어 스테이지보다 낮거나 같은 값을 설정하려고 합니다.");
            return false;
        }
        StageProgress.MaxClearedStage = stage;
        RequestSave(savePolicy);
        return true;
    }

    /// <summary>
    /// upgradeState를 받아서 AirshipSaveData에 반영하는 메서드
    /// </summary>
    /// <param name="upgradeState"></param>
    /// <param name="savePolicy"></param>

    public void SetAirshipUpgradeState(AirshipUpgradeState upgradeState, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized()) return;
        if (upgradeState == null) return;

        Airship.AttackLevel = upgradeState.AttackLevel;
        Airship.DefenseLevel = upgradeState.DefenseLevel;
        Airship.MaxHealthLevel = upgradeState.MaxHealthLevel;
        Airship.CriticalLevel = upgradeState.CriticalLevel;

        RequestSave(savePolicy);
    }

    /// <summary>
    /// 장착된 캐논 ID를 세이브 데이터에 반영하는 메서드
    /// </summary>
    /// <param name="cannonType"></param>
    /// <param name="savePolicy"></param>
    public void SetEquippedCannonId(AirshipCannonType cannonType, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized()) return;
        Airship.EquippedCannonType = cannonType;
        RequestSave(savePolicy);
    }

    /// <summary>
    /// 장착된 기어 ID를 세이브 데이터에 반영하는 메서드
    /// </summary>
    /// <param name="gearType"></param>
    /// <param name="savePolicy"></param>
    public void SetEquippedGearId(AirshipGearType gearType, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized()) return;
        Airship.EquippedGearType = gearType;
        RequestSave(savePolicy);
    }

    public bool TryGetHeroData(string heroName, out HeroSaveData heroData)
    {
        heroData = null;

        if (!CheckInitialized()) return false;
        if (string.IsNullOrWhiteSpace(heroName)) return false;
        if (Heroes == null) return false;

        return Heroes.TryGetValue(heroName, out heroData);
    }

    public bool IsHeroOwned(string heroName)
    {
        return TryGetHeroData(heroName, out HeroSaveData heroData)
            && heroData.IsOwned;
    }

    public bool SetHeroOwned(string heroName, bool isOwned, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!TryGetHeroData(heroName, out HeroSaveData heroData)) return false;
        if (heroData.IsOwned == isOwned) return true;

        heroData.IsOwned = isOwned;
        OnHeroOwnedChanged?.Invoke(heroName, isOwned);
        RequestSave(savePolicy);
        return true;
    }

    public bool SetHeroLevel(string heroName, int level, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!TryGetHeroData(heroName, out HeroSaveData heroData)) return false;
        if (level < 1) return false;
        if (heroData.Level == level) return true;

        heroData.Level = level;
        RequestSave(savePolicy);
        return true;
    }

    private bool CheckInitialized()
    {
        if (!IsInitialized)
        {
            Debug.LogError("PlayerInfo가 초기화되지 않았습니다.");
            return false;
        }
        return true;
    }

    private void RequestSave(SavePolicy savePolicy)
    {
        if (SaveScheduler.Instance == null)
        {
            Debug.LogWarning("SaveScheduler가 없어 저장 요청을 수행하지 못했습니다.");
            return;
        }

        SaveScheduler.Instance.RequestSave(savePolicy);
    }
}

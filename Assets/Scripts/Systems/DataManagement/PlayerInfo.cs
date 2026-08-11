using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private HeroCatalog heroCatalog;

    private static PlayerInfo instance;

    public static PlayerInfo Instance
    {
        get
        {
            if (instance == null)
            {
                PlayerInfo info = FindFirstObjectByType<PlayerInfo>();
                if (info == null)
                {
                    GameObject obj = new GameObject("PlayerInfo");
                    instance = obj.AddComponent<PlayerInfo>();
                }
                else
                {
                    instance = info;
                }
            }

            instance.Initialize();
            return instance;
        }
    }

    public PlayerSaveData SaveData { get; private set; }
    public bool IsInitialized { get; private set; }

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

    public void Initialize()
    {
        if (IsInitialized) return;
        
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

        SaveScheduler.Instance.Initialize(SaveData, saveDataWriter);

        IsInitialized = true;
    }

    // 세이브 데이터 조회를 위한 프로퍼티
    public PlayerProfileSaveData Profile => SaveData.Profile;
    public AirshipSaveData Airship => SaveData.Airship;
    public StageProgressSaveData StageProgress => SaveData.StageProgress;
    public WalletSaveData Wallet => SaveData.Wallet;

    public int CurrentStage => SaveData.StageProgress.CurrentStage;
    public int MaxClearedStage => SaveData.StageProgress.MaxClearedStage;

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

        SaveScheduler.Instance.RequestSave(savePolicy);
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
        SaveScheduler.Instance.RequestSave(savePolicy);
        return true;
    }

    /// <summary>
    /// 스테이지 진행 상황 업데이트 메서드
    /// </summary>
    /// <param name="stage">갱신하고자하는 스테이지 번호</param>
    /// <param name="savePolicy">저장 정책. 기본 설정은 Soon으로 스테이지 설정 변경 시 약 5초 후에 저장됨.</param>

    public void SetCurrentStage(int stage, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized()) return;
        if (stage < 1)
        {
            Debug.LogError("SetCurrentStage: stage는 1 이상이어야 합니다.");
            return;
        }
        if (stage == SaveData.StageProgress.CurrentStage)
        {
            Debug.LogWarning("SetCurrentStage: 현재 스테이지와 동일한 값을 설정하려고 합니다.");
            return;
        }

        StageProgress.CurrentStage = stage;

        SaveScheduler.Instance.RequestSave(savePolicy);
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
        SaveScheduler.Instance.RequestSave(savePolicy);
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
}

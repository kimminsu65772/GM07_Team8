using System;
using System.Collections.Generic;
using System.IO;
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
    public event Action OnEquipmentInventoryChanged;
    public bool IsInitialized { get; private set; }

    public event Action<CurrencyType> OnCurrencyChanged;

    public event Action OnItemAmountChanged;

    // UI에서 캐싱한 Hero 목록 상태를 갱신할 수 있도록 Hero 소유 여부 변경 시 이벤트 발생
    public event Action<HeroNameEnum, bool> OnHeroOwnedChanged;

    // 영웅이 착용한 장비 스탯이 바로 적용될 수 있도록 장비 착용/해제 시 이벤트 발생
    public event Action<HeroNameEnum> OnHeroEquippedChanged;

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
        Debug.Log($"PlayerInfo: SaveDataPath = {saveDataPath}");
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

        bool isMigratedSaveData = MigrateSaveDataIfNeeded();

        Debug.Log(saveDataPath);

        SaveScheduler.Instance.Initialize(SaveData, saveDataWriter);

        bool isAddedNewHero = false;

        foreach (HeroEntry entry in heroCatalog.InGameHeroEntries)
        {
            if (!SaveData.Heroes.ContainsKey(entry.HeroId))
            {
                SaveData.Heroes[entry.HeroId] = new HeroSaveData
                {
                    IsOwned = entry.IsDefaultOwned,
                    Level = entry.DefaultLevel
                };

                isAddedNewHero = true;
            }
        }

        if (isAddedNewHero || isMigratedSaveData)
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
    public bool RepeatClearedStage => SaveData.StageProgress.RepeatClearedStage;
    public Dictionary<HeroNameEnum, HeroSaveData> Heroes => SaveData.Heroes;

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
    public void AddCurrency(CurrencyType type, long amount, SavePolicy savePolicy = SavePolicy.Deferred)
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

    public bool TrySpendCurrency(CurrencyType type, long amount, SavePolicy savePolicy = SavePolicy.Soon)
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

    public bool SetRepeatClearedStage(bool repeatClearedStage, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized()) return false;
        if (StageProgress.RepeatClearedStage == repeatClearedStage) return true;

        StageProgress.RepeatClearedStage = repeatClearedStage;
        RequestSave(savePolicy);
        return true;
    }

    public int GetAirshipUpgradeLevel(AirshipStatType statType)
    {
        if (!CheckInitialized()) return -1;

        return statType switch
        {
            AirshipStatType.Attack => Airship.AttackLevel,
            AirshipStatType.Recovery => Airship.RecoveryLevel,
            AirshipStatType.MaxHealth => Airship.MaxHealthLevel,
            AirshipStatType.CriticalChance => Airship.CriticalLevel,
            _ => -1
        };
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
        Airship.RecoveryLevel = upgradeState.RecoveryLevel;
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

    public bool IsCannonOwned(AirshipCannonType cannonType)
    {
        if (!CheckInitialized()) return false;
        if (Airship.OwnedCannons == null) return false;
        return Airship.OwnedCannons.Contains(cannonType);
    }

    public bool IsGearOwned(AirshipGearType gearType)
    {
        if (!CheckInitialized()) return false;
        if (Airship.OwnedGears == null) return false;
        return Airship.OwnedGears.Contains(gearType);
    }

    /// <summary>
    /// 캐논을 해금할 때 세이브 데이터 소유 캐논 목록에 추가하는 메서드
    /// </summary>
    /// <param name="cannonType"></param>
    /// <param name="savePolicy"></param>
    public void SetOwnedCannonId(AirshipCannonType cannonType, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized()) return;
        Airship.OwnedCannons ??= new HashSet<AirshipCannonType>();
        if (!Airship.OwnedCannons.Add(cannonType))
        {
            Debug.LogWarning($"SetOwnedCannonId: 이미 소유한 캐논입니다. CannonType: {cannonType}");
            return;
        }
        Debug.Log($"SetOwnedCannonId: 캐논 해금 완료. CannonType: {cannonType}");
        RequestSave(savePolicy);
    }

    /// <summary>
    /// 캐논을 해금할 때 세이브 데이터 소유 캐논 목록에 추가하는 메서드
    /// </summary>
    /// <param name="gearType"></param>
    /// <param name="savePolicy"></param>
    public void SetOwnedGearId(AirshipGearType gearType, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized()) return;
        Airship.OwnedGears ??= new HashSet<AirshipGearType>();
        if (!Airship.OwnedGears.Add(gearType))
        {
            Debug.LogWarning($"SetOwnedGearId: 이미 소유한 기어입니다. GearType: {gearType}");
            return;
        }
        Debug.Log($"SetOwnedGearId: 기어 해금 완료. GearType: {gearType}");
        RequestSave(savePolicy);
    }

    public bool TryGetHeroData(HeroNameEnum heroId, out HeroSaveData heroData)
    {
        heroData = null;

        if (!CheckInitialized()) return false;
        if (heroId == HeroNameEnum.None) return false;
        if (Heroes == null) return false;

        return Heroes.TryGetValue(heroId, out heroData);
    }

    public bool IsHeroOwned(HeroNameEnum heroId)
    {
        return TryGetHeroData(heroId, out HeroSaveData heroData)
            && heroData.IsOwned;
    }

    public bool SetHeroOwned(HeroNameEnum heroId, bool isOwned, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!TryGetHeroData(heroId, out HeroSaveData heroData)) return false;
        if (heroData.IsOwned == isOwned) return true;

        heroData.IsOwned = isOwned;
        OnHeroOwnedChanged?.Invoke(heroId, isOwned);
        RequestSave(savePolicy);
        return true;
    }

    public bool SetHeroLevel(HeroNameEnum heroId, int level, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!TryGetHeroData(heroId, out HeroSaveData heroData)) return false;
        if (level < 1) return false;
        if (heroData.Level == level) return true;

        heroData.Level = level;
        RequestSave(savePolicy);
        return true;
    }

    public int GetItemAmount(int itemId)
    {
        if (!CheckInitialized() || SaveData == null) return 0;

        // 세이브 데이터는 보유중인 아이템 Id만 들고 있어서 패치를 통해 인게임에 추가된 재료가
        // 세이브 데이터에는 없을 수도 있음. 따라서 이 경우에는 0을 반환.
        return SaveData.Inventory.Items.TryGetValue(itemId, out ItemStackSaveData item)
            ? item.Amount
            : 0;
    }

    public bool HasEnoughItem(int itemId, int amount)
    {
        if (amount <= 0) return true;
        int currentAmount = GetItemAmount(itemId);

        return currentAmount >= amount;
    }

    public bool TryConsumeItem(int itemId, int amount, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized() || SaveData == null || amount <= 0) return false;
        if (!SaveData.Inventory.Items.ContainsKey(itemId)) return false;

        if (HasEnoughItem(itemId, amount))
        {
            SaveData.Inventory.Items[itemId].Amount -= amount;
            OnItemAmountChanged?.Invoke();
            RequestSave(savePolicy);
            return true;
        }

        return false;
    }

    public void AddItem(int itemId, int amount, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized() || SaveData == null || amount <= 0) return;
        if (!SaveData.Inventory.Items.TryGetValue(itemId, out ItemStackSaveData item))
        {
            item = new ItemStackSaveData();
            SaveData.Inventory.Items[itemId] = item;
        }
        SaveData.Inventory.Items[itemId].Amount += amount;
        OnItemAmountChanged?.Invoke();
        RequestSave(savePolicy);
    }

    public IReadOnlyList<EquipmentSaveData> GetOwnedEquipments()
    {
        if (!CheckInitialized() || SaveData == null)
            return Array.Empty<EquipmentSaveData>();

        EnsureEquipmentInventoryData();
        return SaveData.EquipmentInventory.Equipments;
    }

    public IReadOnlyList<int> GetOwnedEquipmentIds()
    {
        if (!CheckInitialized() || SaveData == null)
            return Array.Empty<int>();

        EnsureEquipmentInventoryData();

        List<int> equipmentIds = new();
        foreach (EquipmentSaveData equipment in SaveData.EquipmentInventory.Equipments)
        {
            if (equipment != null)
            {
                equipmentIds.Add(equipment.EquipId);
            }
        }

        return equipmentIds;
    }

    public bool TryGetEquipment(int equipmentId, out EquipmentSaveData equipment)
    {
        equipment = null;

        if (!CheckInitialized() || SaveData == null) return false;
        if (equipmentId <= 0) return false;

        EnsureEquipmentInventoryData();

        foreach (EquipmentSaveData ownedEquipment in SaveData.EquipmentInventory.Equipments)
        {
            if (ownedEquipment != null && ownedEquipment.EquipId == equipmentId)
            {
                equipment = ownedEquipment;
                return true;
            }
        }

        return false;
    }

    public void GetHeroEquippedEquipments(HeroNameEnum heroId, 
        out EquipmentSaveData weapon, 
        out EquipmentSaveData armor, 
        out EquipmentSaveData acc)
    {
        weapon = null;
        armor = null;
        acc = null;
        if (!CheckInitialized() || SaveData == null) return;
        if (!SaveData.Heroes.TryGetValue(heroId, out HeroSaveData heroData)) return;
        if (heroData == null) return;

        weapon = heroData.EquippedWeaponId > 0 && TryGetEquipment(heroData.EquippedWeaponId, out EquipmentSaveData weaponData) ? weaponData : null;
        armor = heroData.EquippedBodyId > 0 && TryGetEquipment(heroData.EquippedBodyId, out EquipmentSaveData armorData) ? armorData : null;
        acc = heroData.EquippedAccId > 0 && TryGetEquipment(heroData.EquippedAccId, out EquipmentSaveData accData) ? accData : null;
    }

    public bool HasEquipment(int equipmentId)
    {
        return TryGetEquipment(equipmentId, out _);
    }

    public bool AddEquipment(EquipmentSaveData equipment, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized() || SaveData == null) return false;
        if (equipment == null) return false;

        EnsureEquipmentInventoryData();

        if (equipment.EquipId <= 0)
        {
            equipment.EquipId = GetNextEquipId();
        }

        if (HasEquipment(equipment.EquipId))
            return false;

        SaveData.EquipmentInventory.Equipments.Add(equipment);

        if (equipment.EquipId >= SaveData.EquipmentInventory.NextEquipId)
        {
            SaveData.EquipmentInventory.NextEquipId = equipment.EquipId + 1;
        }

        RequestSave(savePolicy);
        OnEquipmentInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveEquipment(int equipmentId, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized() || SaveData == null) return false;
        if (equipmentId <= 0) return false;

        EnsureEquipmentInventoryData();

        bool removed = false;
        for (int i = SaveData.EquipmentInventory.Equipments.Count - 1; i >= 0; i--)
        {
            EquipmentSaveData equipment = SaveData.EquipmentInventory.Equipments[i];
            if (equipment != null && equipment.EquipId == equipmentId)
            {
                SaveData.EquipmentInventory.Equipments.RemoveAt(i);
                removed = true;
                break;
            }
        }

        if (removed)
        {
            RequestSave(savePolicy);
            OnEquipmentInventoryChanged?.Invoke();
        }

        return removed;
    }

    public int GetHeroEquippedId(HeroNameEnum heroId, EquipPartEnum equipPart)
    {
        if (!CheckInitialized() || SaveData == null) return 0;
        if (!SaveData.Heroes.TryGetValue(heroId, out HeroSaveData heroData)) return 0;
        if (heroData == null) return 0;

        return equipPart switch
        {
            EquipPartEnum.Weapon => heroData.EquippedWeaponId,
            EquipPartEnum.Body => heroData.EquippedBodyId,
            EquipPartEnum.Acc => heroData.EquippedAccId,
            _ => 0
        };
    }

    public bool TryGetEquippedHero(int equipmentId, out HeroNameEnum heroId)
    {
        heroId = HeroNameEnum.None;

        if (!CheckInitialized() || SaveData == null) return false;
        if (equipmentId <= 0) return false;

        foreach ((HeroNameEnum currentHeroId, HeroSaveData heroData) in SaveData.Heroes)
        {
            if (heroData == null)
            {
                continue;
            }

            if (heroData.EquippedWeaponId == equipmentId ||
                heroData.EquippedBodyId == equipmentId ||
                heroData.EquippedAccId == equipmentId)
            {
                heroId = currentHeroId;
                return true;
            }
        }

        return false;
    }

    public bool SetHeroEquippedEquipmentId(HeroNameEnum heroId, EquipPartEnum equipPart, int equipmentId, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized() || SaveData == null) return false;
        if (!SaveData.Heroes.TryGetValue(heroId, out HeroSaveData heroData)) return false;
        if (heroData == null) return false;
        if (equipmentId <= 0) return false;
        if (!HasEquipment(equipmentId)) return false;
        if (GetHeroEquippedId(heroId, equipPart) == equipmentId) return true;


        switch (equipPart)
        {
            case EquipPartEnum.Weapon:
                heroData.EquippedWeaponId = equipmentId;
                break;
            case EquipPartEnum.Body:
                heroData.EquippedBodyId = equipmentId;
                break;
            case EquipPartEnum.Acc:
                heroData.EquippedAccId = equipmentId;
                break;
            default:
                return false;
        }

        OnHeroEquippedChanged?.Invoke(heroId);
        RequestSave(savePolicy);
        return true;
    }

    public bool ClearHeroEquippedEquipmentId(HeroNameEnum heroId, EquipPartEnum equipPart, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized() || SaveData == null) return false;
        if (!SaveData.Heroes.TryGetValue(heroId, out HeroSaveData heroData)) return false;
        if (heroData == null) return false;

        switch (equipPart)
        {
            case EquipPartEnum.Weapon:
                heroData.EquippedWeaponId = 0;
                break;

            case EquipPartEnum.Body:
                heroData.EquippedBodyId = 0;
                break;

            case EquipPartEnum.Acc:
                heroData.EquippedAccId = 0;
                break;

            default:
                return false;
        }

        OnHeroEquippedChanged?.Invoke(heroId);
        RequestSave(savePolicy);
        return true;
    }

    public EquipmentInventorySaveData GetOwnedEquips()
    {
        if (!CheckInitialized() || SaveData == null)
            return new EquipmentInventorySaveData
            {
                NextEquipId = 1,
                Equipments = new List<EquipmentSaveData>()
            };

        EnsureEquipmentInventoryData();
        return SaveData.EquipmentInventory;
    }

    private void EnsureEquipmentInventoryData()
    {
        if (SaveData.EquipmentInventory == null)
        {
            SaveData.EquipmentInventory = new EquipmentInventorySaveData
            {
                NextEquipId = 1,
                Equipments = new List<EquipmentSaveData>()
            };
        }
    }

    public int GetNextEquipId()
    {
        EnsureEquipmentInventoryData();

        int nextId = SaveData.EquipmentInventory.NextEquipId;
        return nextId;
    }

    public IReadOnlyList<EquipmentCraftSlotSaveData> GetEquipmentCraftSlots()
    {
        if (!CheckInitialized() || SaveData == null)
            return Array.Empty<EquipmentCraftSlotSaveData>();

        return SaveData.EquipmentCraft.Slots;
    }

    public EquipmentCraftSlotSaveData GetEquipmentCraftSlot(int slotIndex)
    {
        if (!CheckInitialized() || SaveData == null) return null;

        foreach (var slot in SaveData.EquipmentCraft.Slots)
        {
            if (slot.SlotIndex == slotIndex) 
                return slot;
        }

        return null;
    }

    public bool StartEquipmentCraft(int slotIndex, int recipeId, DateTime startedAtUtc, DateTime completesAtUtc, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized() || SaveData == null) return false;
        if (recipeId < 0) return false;
        if (completesAtUtc <= startedAtUtc) return false;

        EquipmentCraftSlotSaveData slot = GetEquipmentCraftSlot(slotIndex);

        if (slot == null) return false;
        if (slot.IsCrafting) return false;

        slot.IsCrafting = true;
        slot.RecipeId = recipeId;
        slot.StartedAtUtc = startedAtUtc.ToString("o");
        slot.CompletesAtUtc = completesAtUtc.ToString("o");

        RequestSave(savePolicy);
        return true;
    }

    public bool IsEquipmentCraftComplete(int slotIndex, DateTime nowUtc)
    {
        EquipmentCraftSlotSaveData slot = GetEquipmentCraftSlot(slotIndex);

        if (slot == null) return false;
        if (!slot.IsCrafting) return false;
        if (string.IsNullOrEmpty(slot.CompletesAtUtc)) return false;

        // string으로 저장된 완료 시간을 다시 국제 표준 시간 기준 DateTime 형식으로 변경
        if (!DateTime.TryParse(slot.CompletesAtUtc, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime completesAtUtc))
        {
            return false;
        }

        return nowUtc >= completesAtUtc;
    }

    public bool ClearEquipmentCraftSlot(int slotIndex, SavePolicy savePolicy = SavePolicy.Soon)
    {
        if (!CheckInitialized() || SaveData == null) return false;

        EquipmentCraftSlotSaveData slot = GetEquipmentCraftSlot(slotIndex);

        if (slot == null)
            return false;

        slot.IsCrafting = false;
        slot.RecipeId = 0;
        slot.StartedAtUtc = string.Empty;
        slot.CompletesAtUtc = string.Empty;

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

    /*
     * 세이브 데이터 동기화 메서드 영역
     */

    // v1: 초기 세이브 구조
    // v2: 스테이지 반복 설정 RepeatClearedStage 추가
    // v3: 영웅/편성 저장 키 string → HeroNameEnum 마이그레이션 (Json을 수정해야 해서 Loader에 구현)
    // v4: 비행선 파츠 소유 데이터 추가 (기본 장비를 소유 목록에 넣지 않는 에러 발견)
    // v5: 비행선 파츠 소유 HashSet 초기화/마이그레이션 보정
    // v6: 아이템 인벤토리, 장비 인벤토리, 영웅 장비 착용 상태, 장비 제작 진행 상태 추가
    // 장비 데이터 저장 방식이 아예 바뀌어서 의미가 있나 싶다...
    private bool MigrateSaveDataIfNeeded()
    {
        if (SaveData.SaveVersion >= SaveDataVersion.CurrentVersion) return false;

        if (SaveData.SaveVersion < 2)
        {
            SaveData.StageProgress.RepeatClearedStage = true;
        }

        if (SaveData.SaveVersion < 5)
        {
            MigrateAirshipPartsOwnership();
        }

        if (SaveData.SaveVersion < 6)
        {
            MigrateInventoryAndEquipmentCraft();
        }

        SaveData.SaveVersion = SaveDataVersion.CurrentVersion;

        return true;
    }

    private void MigrateAirshipPartsOwnership()
    {
        if (SaveData?.Airship == null) return;

        // 기존 세이브 데이터에 없는 비행선 파츠 소유 여부를 초기화하여 세이브 데이터에 반영할 수 있도록 한다.
        SaveData.Airship.OwnedCannons ??= new HashSet<AirshipCannonType>();
        SaveData.Airship.OwnedGears ??= new HashSet<AirshipGearType>();

        SaveData.Airship.OwnedCannons.Add(AirshipCannonType.Normal);
        SaveData.Airship.OwnedGears.Add(AirshipGearType.Normal);

        // 테스트 과정에서 이미 다른 파츠를 장착한 경우, 해당 파츠도 소유한 것으로 간주하여 OwnedCannons와 OwnedGears에 추가
        SaveData.Airship.OwnedCannons.Add(SaveData.Airship.EquippedCannonType);
        SaveData.Airship.OwnedGears.Add(SaveData.Airship.EquippedGearType);
    }

    // 장비 제작과 제작에 필요한 재료가 추가됨에 따라 인벤토리 및 장비 제작 관련 데이터를 추가하여 구버전 세이브 데이터를 동기화
    private void MigrateInventoryAndEquipmentCraft()
    {
        SaveData.Inventory ??= new InventorySaveData();
        SaveData.Inventory.Items ??= new Dictionary<int, ItemStackSaveData>();

        SaveData.EquipmentInventory ??= new EquipmentInventorySaveData();
        SaveData.EquipmentInventory.Equipments ??= new List<EquipmentSaveData>();

        if (SaveData.EquipmentInventory.NextEquipId <= 0)
        {
            SaveData.EquipmentInventory.NextEquipId = 1;
        }

        SaveData.EquipmentCraft ??= new EquipmentCraftSaveData();
        SaveData.EquipmentCraft.Slots ??= new List<EquipmentCraftSlotSaveData>();

        if (SaveData.EquipmentCraft.Slots.Count == 0)
        {
            SaveData.EquipmentCraft.Slots.Add(new EquipmentCraftSlotSaveData
            {
                SlotIndex = 0,
                IsCrafting = false,
                RecipeId = 0,
                StartedAtUtc = string.Empty,
                CompletesAtUtc = string.Empty
            });
        }
    }

    public async void ResetData()
    {
        if (!CheckInitialized()) return;

        await SaveScheduler.Instance.FlushAsync();

        SaveData = SaveDataFactory.CreateNewData(heroCatalog);
        SaveScheduler.Instance.Initialize(SaveData, saveDataWriter);
        saveDataWriter.ForceSave(SaveData);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/TitleScene");
    }

#if UNITY_EDITOR
    [ContextMenu("Reset Data")]
    private async void ContextResetData()
    {
        if (!CheckInitialized()) return;

        await SaveScheduler.Instance.FlushAsync();

        SaveData = SaveDataFactory.CreateNewData(heroCatalog);
        SaveScheduler.Instance.Initialize(SaveData, saveDataWriter);
        saveDataWriter.ForceSave(SaveData);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/TitleScene");
    }

    [ContextMenu("Test/Add Test Currencies")]
    private void ContextAddTestCurrencies()
    {
        if (!CheckInitialized()) return;
        AddCurrency(CurrencyType.Gold, 1000000, SavePolicy.Deferred);
        AddCurrency(CurrencyType.Gems, 100000, SavePolicy.Deferred);
        AddCurrency(CurrencyType.Gear, 100000, SavePolicy.Deferred);
    }

    [ContextMenu("Test/Add Test Items")]
    private void ContextAddItems()
    {
        if (!CheckInitialized()) return;
        AddItem(10000, 100, SavePolicy.Deferred);
        AddItem(10001, 100, SavePolicy.Deferred);
        AddItem(10002, 100, SavePolicy.Deferred);
    }
    #endif
}

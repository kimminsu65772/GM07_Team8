using System.Collections.Generic;

/// <summary>
/// SaveDataFactory는 게임을 새로 시작할 때 기본적인 세이브 데이터를 생성하는 역할을 한다.
/// 해당 클래스는 세이브/로드 코드 과정에서 기존 데이터가 없을 경우, 기본 데이터를 생성하여 게임을 새롭게 시작할 수 있도록 한다.
/// 세이브/로드와 세이브 데이터 생성에 대한 책임을 분리하여 코드의 유지보수성을 높이고자 하였다.
/// </summary>
public class SaveDataFactory
{
    public static PlayerSaveData CreateNewData(HeroCatalog heroCatalog = null)
    {
        PlayerSaveData newData = new PlayerSaveData
        {
            SaveVersion = SaveDataVersion.CurrentVersion,
            Profile = new PlayerProfileSaveData
            {
                PlayerId = System.Guid.NewGuid().ToString(),
                Nickname = "Player"
            },
            Airship = new AirshipSaveData
            {
                AttackLevel = 1,
                RecoveryLevel = 1,
                MaxHealthLevel = 1,
                CriticalLevel = 1,
                EquippedCannonType = AirshipCannonType.Normal,
                EquippedGearType = AirshipGearType.Normal,
                OwnedCannons = new HashSet<AirshipCannonType> { AirshipCannonType.Normal },
                OwnedGears = new HashSet<AirshipGearType> { AirshipGearType.Normal }
            },
            Heroes = CreateInitialHeroSaveData(heroCatalog),
            HeroFormation = new HeroFormationSaveData
            {
                Slots = new List<HeroSaveSlot>
                {
                    new HeroSaveSlot { SlotIndex = 0, HeroId = HeroNameEnum.Warrior },
                    new HeroSaveSlot { SlotIndex = 1, HeroId = HeroNameEnum.Mage },
                    new HeroSaveSlot { SlotIndex = 2, HeroId = HeroNameEnum.None },
                    new HeroSaveSlot { SlotIndex = 3, HeroId = HeroNameEnum.None },
                    new HeroSaveSlot { SlotIndex = 4, HeroId = HeroNameEnum.None }
                }
            },
            StageProgress = new StageProgressSaveData
            {
                CurrentStage = 1,
                MaxClearedStage = 0,
                RepeatClearedStage = false
            },
            Wallet = new WalletSaveData
            {
                Currencies = new Dictionary<CurrencyType, CurrencySaveData>
                {
                    { CurrencyType.Gold, new CurrencySaveData { Amount = 1000 } },
                    { CurrencyType.Gems, new CurrencySaveData { Amount = 50 } }
                }
            },
            Inventory = new InventorySaveData
            {
                Items = new Dictionary<int, ItemStackSaveData>
                {
                    { 10000, new ItemStackSaveData { Amount = 100 } },
                    { 10001, new ItemStackSaveData { Amount = 100 } },
                    { 10002, new ItemStackSaveData { Amount = 100 } }
                }
            },
            EquipmentInventory = new EquipmentInventorySaveData
            {
                NextEquipId = 1,
                Equipments = new List<EquipmentSaveData>()
            },
            EquipmentCraft = new EquipmentCraftSaveData
            {
                Slots = new List<EquipmentCraftSlotSaveData>
                {
                    new EquipmentCraftSlotSaveData
                    {
                        SlotIndex = 0,
                        IsCrafting = false,
                        RecipeId = 0,
                        StartedAtUtc = string.Empty,
                        CompletesAtUtc = string.Empty
                    }
                }
            },
            AutoSkillEnabled = false,
            // 마지막 저장 시간은 단순히 오프라인 보상 계산이나 기타 시간 기반 로직 수행 시 시간 차이를 계산하기 위함이기 때문에
            // 별도로 한국 시간으로 변환하지 않고 UTC 기준으로 저장한다.
            LastSavedAtUtc = System.DateTime.UtcNow.ToString("o")
        };

        return newData;
    }

    private static Dictionary<HeroNameEnum, HeroSaveData> CreateInitialHeroSaveData(HeroCatalog heroCatalog)
    {
        Dictionary<HeroNameEnum, HeroSaveData> heroes = new Dictionary<HeroNameEnum, HeroSaveData>();

        if (heroCatalog == null)
        {
            return heroes;
        }

        foreach (HeroEntry entry in heroCatalog.InGameHeroEntries)
        {
            heroes[entry.HeroId] = new HeroSaveData
            {
                Level = entry.DefaultLevel,
                IsOwned = entry.IsDefaultOwned,
                EquippedWeaponId = 0,
                EquippedBodyId = 0,
                EquippedAccId = 0
            };
        }

        return heroes;
    }
}

// 나중에 데이터 구조가 변경되었을 때 버전 관리 및 비교 후 보정 작업을 위해 현재 세이브 데이터의 버전을 상수로 정의한다.
public static class SaveDataVersion
{
    public const int CurrentVersion = 8;
}

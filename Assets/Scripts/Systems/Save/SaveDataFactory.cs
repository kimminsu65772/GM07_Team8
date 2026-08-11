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
                AttackLevel = 0,
                DefenseLevel = 0,
                MaxHealthLevel = 0,
                CriticalLevel = 0,
                EquippedCannonId = "NormalCannon",
                EquippedGearId = "MaxHpGear"
            },
            Heroes = CreateInitialHeroSaveData(heroCatalog),
            HeroFormation = new HeroFormationSaveData
            {
                Slots = new List<HeroSaveSlot>
                {
                    new HeroSaveSlot { SlotIndex = 0, HeroName = null },
                    new HeroSaveSlot { SlotIndex = 1, HeroName = null },
                    new HeroSaveSlot { SlotIndex = 2, HeroName = null },
                    new HeroSaveSlot { SlotIndex = 3, HeroName = null },
                    new HeroSaveSlot { SlotIndex = 4, HeroName = null }
                }
            },
            StageProgress = new StageProgressSaveData
            {
                CurrentStage = 1,
                MaxClearedStage = 0
            },
            Wallet = new WalletSaveData
            {
                Currencies = new Dictionary<CurrencyType, CurrencySaveData>
                {
                    { CurrencyType.Gold, new CurrencySaveData { Amount = 1000 } },
                    { CurrencyType.Gems, new CurrencySaveData { Amount = 50 } }
                }
            },
            // 마지막 저장 시간은 단순히 오프라인 보상 계산이나 기타 시간 기반 로직 수행 시 시간 차이를 계산하기 위함이기 때문에
            // 별도로 한국 시간으로 변환하지 않고 UTC 기준으로 저장한다.
            LastSavedAtUtc = System.DateTime.UtcNow.ToString("o")
        };

        return newData;
    }

    private static Dictionary<string, HeroSaveData> CreateInitialHeroSaveData(HeroCatalog heroCatalog)
    {
        Dictionary<string, HeroSaveData> heroes = new Dictionary<string, HeroSaveData>();

        if (heroCatalog == null)
        {
            return heroes;
        }

        foreach (HeroEntry entry in heroCatalog.GetDefaultOwnedHeroEntries())
        {
            heroes[entry.HeroName] = new HeroSaveData
            {
                Level = entry.DefaultLevel,
                IsOwned = true
            };
        }

        return heroes;
    }
}


// 나중에 데이터 구조가 변경되었을 때 버전 관리 및 비교 후 보정 작업을 위해 현재 세이브 데이터의 버전을 상수로 정의한다.
public static class SaveDataVersion
{
    public const int CurrentVersion = 1;
}

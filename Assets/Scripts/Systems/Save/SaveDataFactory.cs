using System.Collections.Generic;

/// <summary>
/// SaveDataFactory는 게임을 새로 시작할 때 기본적인 세이브 데이터를 생성하는 역할을 한다.
/// 해당 클래스는 세이브/로드 코드 과정에서 기존 데이터가 없을 경우, 기본 데이터를 생성하여 게임을 새롭게 시작할 수 있도록 한다.
/// 세이브/로드와 세이브 데이터 생성에 대한 책임을 분리하여 코드의 유지보수성을 높이고자 하였다.
/// </summary>
public class SaveDataFactory
{
    public static PlayerSaveData CreateNewData()
    {
        PlayerSaveData newData = new PlayerSaveData
        {
            SaveVersion = 1,
            Profile = new PlayerProfileSaveData
            {
                PlayerId = System.Guid.NewGuid().ToString(),
                Nickname = "Player"
            },
            Airship = new AirshipSaveData
            {
                Level = 1,
                Experience = 0
            },
            Heroes = new Dictionary<string, HeroSaveData>
            {
                { "BaseKnight", new HeroSaveData { Level = 1, IsOwned = true, FormationType = "Frontline", FormationIndex = 0 } },
                { "BaseArcher", new HeroSaveData { Level = 1, IsOwned = true, FormationType = "Backline", FormationIndex = 0 } },
                { "BaseThief", new HeroSaveData { Level = 1, IsOwned = false, FormationType = "None", FormationIndex = -1 } }
            },
            StageProgress = new StageProgressSaveData
            {
                CurrentStage = 1,
                MaxClearedStage = 0
            },
            Wallet = new WalletSaveData
            {
                Currencies = new Dictionary<string, CurrencySaveData>
                {
                    { "Gold", new CurrencySaveData { Amount = 1000 } },
                    { "Gems", new CurrencySaveData { Amount = 50 } }
                }
            },
            // 마지막 저장 시간은 단순히 오프라인 보상 계산이나 기타 시간 기반 로직 수행 시 시간 차이를 계산하기 위함이기 때문에
            // 별도로 한국 시간으로 변환하지 않고 UTC 기준으로 저장한다.
            LastSavedAtUtc = System.DateTime.UtcNow.ToString("o") // ISO 8601 format
        };

        return newData;
    }
}

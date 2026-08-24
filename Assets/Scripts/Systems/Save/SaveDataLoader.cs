using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

public class SaveDataLoader
{
    private readonly string saveFilePath;
    public SaveDataLoader(string saveFilePath)
    {
        if (string.IsNullOrWhiteSpace(saveFilePath))
        {
            throw new ArgumentException("저장 파일 경로가 비어있거나 공백으로만 이루어져 있습니다.", nameof(saveFilePath));
        }
        this.saveFilePath = saveFilePath;
    }

    // 파일의 유무를 확인하는 메서드
    public bool Exists()
    {
        return File.Exists(saveFilePath);
    }

    public PlayerSaveData Load()
    {
        if (!Exists())
        {
            throw new FileNotFoundException($"저장 파일을 찾을 수 없습니다: {saveFilePath}");
        }

        // 저장 파일에서 JSON 데이터를 읽어온다.
        string json = File.ReadAllText(saveFilePath);

        PlayerSaveData saveData;
        // JSON 데이터를 역직렬화하여 PlayerSaveData 객체로 변환한다.
        // 이때 형식이 잘못되거나, 역직렬화를 실패할 수 있으므로 예외를 처리한다.
        try
        {
            json = MigrateJsonIfNeeded(json);
            saveData = JsonConvert.DeserializeObject<PlayerSaveData>(json);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("JSON 변환 도중 오류가 발생했습니다.", ex);
        }

        // 역직렬화 성공 후에도 해당 데이터가 null일 수 있으므로, null 체크를 수행한다.
        if (saveData == null)
        {
            throw new InvalidOperationException("저장 파일이 유효하지 않아 로드할 수 없습니다.");
        }

        // 데이터 저장 구조가 변경되었을 경우를 대비하여, 현재 세이브 데이터의 버전과 비교하여 필요한 보정 작업을 수행해야함.
        // 데이터 저장 구조 확정 후 DataMigration 메서드 구현 예정

        return saveData;
    }

    private static string MigrateJsonIfNeeded(string json)
    {
        // JSON 데이터를 JObject로 파싱하여 수정가능한 형태로 만든다.
        JObject root = JObject.Parse(json);

        // json 데이터에서 SaveVersion이라는 항목의 값을 가져온다.
        int saveVersion = root["SaveVersion"]?.Value<int>() ?? 0;

        // string 방식에서 enum 방식으로 바뀐 버전이 세이브 버전 3이므로, 세이브 버전이 3보다 낮으면 마이그레이션을 수행한다.
        if (saveVersion < 3)
        {
            MigrateHeroes(root);
            MigrateHeroFormation(root);
            root["SaveVersion"] = 3;
        }

        if (saveVersion < 7)
        {
            MigrateHeroNumericKeys(root);
            MigrateHeroFormationNumericIds(root);
        }

        return root.ToString(Formatting.None);
    }

    private static void MigrateHeroes(JObject root)
    {
        // Heroes 항목을 가져와서 JObject로 변환한다.
        JObject heroes = root["Heroes"] as JObject;

        // 만약 Heroes 항목이 없으면 null이 반환되므로, null 체크를 수행한다.
        if (heroes == null)
        {
            return;
        }

        // 마이그레이션된 영웅 데이터를 담을 새로운 JObject를 생성한다.
        JObject migratedHeroes = new JObject();

        // Properties() 메서드를 사용하여 Heroes 항목의 각 속성을 순회한다.
        foreach (JProperty heroProperty in heroes.Properties())
        {
            string migratedHeroName = ConvertLegacyHeroName(heroProperty.Name);

            if (migratedHeroName == nameof(HeroNameEnum.None))
            {
                continue;
            }

            migratedHeroes[migratedHeroName] = heroProperty.Value;
        }

        root["Heroes"] = migratedHeroes;
    }

    private static void MigrateHeroFormation(JObject root)
    {
        // HeroFormation 항목을 가져와서 JObject로 변환한다.
        JObject heroFormation = root["HeroFormation"] as JObject;

        // Slots 항목은 배열 형태이므로 JArray로 변환하여 가져온다.
        JArray slots = heroFormation?["Slots"] as JArray;

        if (slots == null)
        {
            return;
        }

        // 슬롯을 순회하면서 string 방식의 HeroName을 enum 방식의 HeroId로 변환한다.
        foreach (JObject slot in slots)
        {
            string legacyHeroName = slot["HeroName"]?.Value<string>();
            string migratedHeroName = ConvertLegacyHeroName(legacyHeroName);

            slot["HeroId"] = migratedHeroName;
            slot.Remove("HeroName");
        }
    }

    private static void MigrateHeroFormationNumericIds(JObject root)
    {
        JObject heroFormation = root["HeroFormation"] as JObject;
        JArray slots = heroFormation?["Slots"] as JArray;

        if (slots == null)
        {
            return;
        }

        foreach (JObject slot in slots)
        {
            JToken heroIdToken = slot["HeroId"];
            string migratedHeroName = ConvertLegacyHeroId(heroIdToken);

            if (!string.IsNullOrEmpty(migratedHeroName))
            {
                slot["HeroId"] = migratedHeroName;
            }
        }
    }

    private static void MigrateHeroNumericKeys(JObject root)
    {
        JObject heroes = root["Heroes"] as JObject;

        if (heroes == null)
        {
            return;
        }

        JObject migratedHeroes = new JObject();

        foreach (JProperty heroProperty in heroes.Properties())
        {
            bool isNumericKey = int.TryParse(heroProperty.Name, out _);
            string migratedHeroName = ConvertLegacyHeroId(heroProperty.Name);

            if (migratedHeroName == nameof(HeroNameEnum.None))
            {
                continue;
            }

            if (!isNumericKey || !migratedHeroes.ContainsKey(migratedHeroName))
            {
                migratedHeroes[migratedHeroName] = heroProperty.Value;
            }
        }

        root["Heroes"] = migratedHeroes;
    }

    private static string ConvertLegacyHeroId(string heroIdText)
    {
        if (int.TryParse(heroIdText, out int numericHeroId))
        {
            return numericHeroId switch
            {
                0 => nameof(HeroNameEnum.None),
                1 or 11 => nameof(HeroNameEnum.Warrior),
                2 or 21 => nameof(HeroNameEnum.Mage),
                3 or 22 => nameof(HeroNameEnum.Sorcery),
                _ => nameof(HeroNameEnum.None)
            };
        }

        return heroIdText;
    }

    private static string ConvertLegacyHeroName(string heroName)
    {
        return heroName switch
        {
            "Hero1" => nameof(HeroNameEnum.Warrior),
            "War1" => nameof(HeroNameEnum.Warrior),
            "Warrior" => nameof(HeroNameEnum.Warrior),

            "Hero2" => nameof(HeroNameEnum.Mage),
            "War2" => nameof(HeroNameEnum.Mage),
            "Mage" => nameof(HeroNameEnum.Mage),

            "Sorcery" => nameof(HeroNameEnum.Sorcery),

            null => nameof(HeroNameEnum.None),
            "" => nameof(HeroNameEnum.None),

            _ => nameof(HeroNameEnum.None)
        };
    }

    private static string ConvertLegacyHeroId(JToken heroIdToken)
    {
        if (heroIdToken == null)
        {
            return nameof(HeroNameEnum.None);
        }

        if (heroIdToken.Type == JTokenType.Integer)
        {
            int heroId = heroIdToken.Value<int>();
            return heroId switch
            {
                0 => nameof(HeroNameEnum.None),
                1 or 11 => nameof(HeroNameEnum.Warrior),
                2 or 21 => nameof(HeroNameEnum.Mage),
                3 or 22 => nameof(HeroNameEnum.Sorcery),
                _ => nameof(HeroNameEnum.None)
            };
        }

        return ConvertLegacyHeroId(heroIdToken.Value<string>());
    }
}

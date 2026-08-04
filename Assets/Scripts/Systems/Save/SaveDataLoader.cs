using Newtonsoft.Json;
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
}
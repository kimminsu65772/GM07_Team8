using System;
using System.IO;
using Newtonsoft.Json;

/// <summary>
/// SaveDataWriter는 게임 데이터를 JSON 형식으로 직렬화하여 파일에 저장하는 역할을 한다.
/// </summary>
public class SaveDataWriter
{
    // 저장 파일 경로를 생성자에서 받아서 읽기 전용으로 저장한다.
    private readonly string saveFilePath;
    public SaveDataWriter(string saveFilePath)
    {
        if (string.IsNullOrWhiteSpace(saveFilePath))
        {
            throw new ArgumentException("저장 파일 경로가 비어있거나 공백으로만 이루어져 있습니다.", nameof(saveFilePath));
        }
        this.saveFilePath = saveFilePath;
    }
    
    public void Save(PlayerSaveData saveData)
    {
        // 잘못된 저장 데이터가 들어왔을 경우 예외를 발생시켜 더이상 진행하지 않도록 한다.
        if (saveData == null)
        {
            throw new ArgumentNullException(nameof(saveData), "저장하려는 세이브 데이터가 비어있는 상태입니다.");
        }

        saveData.LastSavedAtUtc = DateTime.UtcNow.ToString("o");

        // 게임 데이터를 직렬화하여 JSON 형식으로 변환한다.
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);

        // 게임 데이터 파일을 제외한 폴더 경로를 조회한다.
        string directoryPath = Path.GetDirectoryName(saveFilePath);

        // 주어진 경로 주소에 상위 폴더가 포함된 경우라면, 해당 경로에 폴더 생성을 시도하여 저장 폴더의 존재를 보장한다.
        // 이 때 이미 폴더가 존재한다면 Directory.CreateDirectory 메서드는 아무런 동작도 하지 않는다.
        if (!string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 기존 파일에 안전하게 데이터를 덮어쓸 수 있도록 따로 임시 파일을 생성한다.
        string tempFilePath = saveFilePath + ".tmp";

        // 임시 파일에 JSON 데이터를 작성한다.
        File.WriteAllText(tempFilePath, json);

        // 기존 저장 파일이 있다면 임시 저장 데이터를 기존 파일록 교체한다.
        if (File.Exists(saveFilePath))
        {
            File.Replace(tempFilePath, saveFilePath, null);
        }
        // 기존 저장 파일이 없다면 임시 저장 데이터 파일을 해당 경로에 옮기고 이름을 변경하여 최종 저장 파일로 만든다.
        else
        {
            File.Move(tempFilePath, saveFilePath);
        }
    }
}

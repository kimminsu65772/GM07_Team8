using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

/// <summary>
/// SaveDataWriter는 게임 데이터를 JSON 형식으로 직렬화하여 파일에 저장하는 역할을 한다.
/// </summary>
public class SaveDataWriter
{
    // 저장 파일 경로를 생성자에서 받아서 읽기 전용으로 저장한다.
    private readonly string saveFilePath;

    // 저장 방식을 비동기 저장과 동기 저장으로 나누었으므로 두 작업이 충돌나지 않도록 하기 위해 lock 객체를 생성한다.
    private readonly object saveLock = new object();
    public SaveDataWriter(string saveFilePath)
    {
        if (string.IsNullOrWhiteSpace(saveFilePath))
        {
            throw new ArgumentException("저장 파일 경로가 비어있거나 공백으로만 이루어져 있습니다.", nameof(saveFilePath));
        }
        this.saveFilePath = saveFilePath;
    }
    
    public UniTask SaveFileAsync(string json)
    {
        // 파일 쓰기 작업을 별도의 스레드에서 비동기적으로 수행한다.
        return UniTask.RunOnThreadPool(() => SaveToFile(json));
    }

    public void ForceSave(PlayerSaveData data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "저장할 데이터가 비어있습니다.");
        }

        data.LastSavedAtUtc = DateTime.UtcNow.ToString("o");
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        SaveToFile(json);
    }

    private void SaveToFile(string json)
    {
        CheckValidateJson(json);
        // lock을 사용하여 동기화된 블록을 생성한다.
        // 이를 통해 동시에 파일 쓰기 작업이 발생하지 않도록 보장한다.
        lock (saveLock)
        {
            string directoryPath = Path.GetDirectoryName(saveFilePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string tempFilePath = saveFilePath + ".tmp";
            string backupFilePath = saveFilePath + ".bak";

            File.WriteAllText(tempFilePath, json);

            if (File.Exists(saveFilePath))
            {
                File.Replace(tempFilePath, saveFilePath, backupFilePath);
            }
            else
            {
                File.Move(tempFilePath, saveFilePath);
            }
        }
    }

    private void CheckValidateJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("저장할 JSON 데이터가 비어있거나 공백으로만 이루어져 있습니다.", nameof(json));
        }
    }
}

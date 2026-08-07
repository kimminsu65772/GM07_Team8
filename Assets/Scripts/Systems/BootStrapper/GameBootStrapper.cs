using System.IO;
using UnityEngine;

public class GameBootStrapper : MonoBehaviour
{
    void Start()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "playerSaveData.json");
        SaveDataLoader saveDataLoader = new SaveDataLoader(saveFilePath);
        SaveDataWriter saveDataWriter = new SaveDataWriter(saveFilePath);

        PlayerSaveData playerSaveData;

        if (saveDataLoader.Exists())
        {
            playerSaveData = saveDataLoader.Load();
            Debug.Log("세이브 데이터를 로드합니다...");
        }
        else
        {
            playerSaveData = SaveDataFactory.CreateNewData();
            saveDataWriter.ForceSave(playerSaveData);
            Debug.Log("세이브 데이터가 없어 새 데이터를 생성합니다.");
        }

        Debug.Log($"세이브 파일 경로: {saveFilePath}");

        RuntimeDataContext runtimeDataContext = new RuntimeDataContext(playerSaveData);
        SaveScheduler.Instance.Initialize(playerSaveData, saveDataWriter);

        OfflineRewardProvider.ProvideOfflineReward(playerSaveData);
    }
}

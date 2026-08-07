using System;
using UnityEngine;

public class StageProgressManager : MonoBehaviour
{
    public static StageProgressManager Instance { get; private set; }

    private StageProgressSaveData stageData;
    private bool isInitialized;

    public bool IsInitialized => isInitialized;
    public int CurrentStage
    {
        get
        {
            CheckInitialized();
            return stageData.CurrentStage;
        }
    }
    public int MaxClearedStage
    {
        get
        {
            CheckInitialized();
            return stageData.MaxClearedStage;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(StageProgressSaveData stageData)
    {
        if (stageData == null)
        {
            throw new ArgumentNullException(nameof(stageData), "스테이지 진행 데이터가 비어 있습니다.");
        }

        this.stageData = stageData;
        isInitialized = true;
    }

    public void SetCurrentStage(int stage)
    {
        CheckInitialized();

        if (stage < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(stage), "스테이지는 0 이하일 수 없습니다.");
        }
        stageData.CurrentStage = stage;
    }

    // 최대 스테이지 갱신은 첫 클리어 보상 지급과 같은 부가적인 로직이 필요할 수 있으므로, 갱신 성공 여부도 반환
    public bool TryUpdateMaxClearedStage(int stage)
    {
        CheckInitialized();

        if (stage < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(stage), "스테이지는 0 이하일 수 없습니다.");
        }
        if (stage > stageData.MaxClearedStage)
        {
            stageData.MaxClearedStage = stage;
            return true;
        }

        return false;
    }

    private void CheckInitialized()
    {
        if (!isInitialized)
        {
            throw new InvalidOperationException("StageProgressManager가 초기화되지 않았습니다.");
        }
    }
}

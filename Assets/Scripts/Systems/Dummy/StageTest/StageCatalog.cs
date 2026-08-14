using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageCatalog", menuName = "Game/Stage/StageCatalog")]
public class StageCatalog : ScriptableObject
{
    [SerializeField] private List<StageData> stages;
    [SerializeField] private int stageCycle = 5;

    private Dictionary<int, StageData> stageDictionary;

    public int StageCount => stages == null ? 0 : stages.Count;

    // 보통은 스테이지 사이클에 맞게 스테이지를 구성하긴 하지만 혹시나 스테이지 사이클에 맞지 않는 스테이지가 존재할 수 있으므로
    // StageCount를 stageCycle로 나눈 값을 올림 처리하여 RegionCount를 계산
    public int RegionCount => stageCycle <= 0 ? 0 : Mathf.CeilToInt((float)StageCount / stageCycle);


    // 스테이지 번호를 통해 StageData를 가져오는 메서드
    public bool TryGetStageData(int stageNumber, out StageData stageData)
    {
        BuildStageDict();
        return stageDictionary.TryGetValue(stageNumber, out stageData);
    }


    // 스테이지 번호에 해당하는 StageData가 존재하는지 확인하는 메서드
    public bool HasStage(int stageNumber)
    {
        BuildStageDict();
        return stageDictionary.ContainsKey(stageNumber);
    }

    private void BuildStageDict()
    {
        if (stageDictionary != null) return;

        stageDictionary = new Dictionary<int, StageData>();

        if (stages == null)
        {
            return;
        }

        foreach (StageData stage in stages)
        {
            if (stage == null)
            {
                continue;
            }

            stageDictionary[stage.StageNumber] = stage;
        }
    }
}

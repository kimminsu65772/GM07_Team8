using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageCatalog", menuName = "Game/Stage/StageCatalog")]
public class StageCatalog : ScriptableObject
{
    [SerializeField] private List<StageData> stages;

    private Dictionary<int, StageData> stageDictionary;

    public int StageCount => stages == null ? 0 : stages.Count;

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

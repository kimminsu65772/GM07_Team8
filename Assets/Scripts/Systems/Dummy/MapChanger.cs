using UnityEngine;

public class MapChanger : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;

    public void NextMap()
    {
        StageProgressManager.Instance.SetCurrentStage(StageProgressManager.Instance.CurrentStage + 1);
        battleManager.SetUpStage(StageProgressManager.Instance.CurrentStage);
        SaveScheduler.Instance.RequestSave(SavePolicy.Soon);
    }

    public void PreviousMap()
    {
        StageProgressManager.Instance.SetCurrentStage(StageProgressManager.Instance.CurrentStage - 1);
        if (StageProgressManager.Instance.CurrentStage > 1)
        {
            battleManager.SetUpStage(StageProgressManager.Instance.CurrentStage);
            SaveScheduler.Instance.RequestSave(SavePolicy.Soon);
        }
        else
        {
            Debug.LogWarning("이미 최소 스테이지에 위치하고 있습니다.");
        }
            
    }
}

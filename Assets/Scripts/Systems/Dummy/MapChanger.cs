using UnityEngine;

public class MapChanger : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;

    public void NextMap()
    {
        int nextStage = PlayerInfo.Instance.CurrentStage + 1;
        PlayerInfo.Instance.SetCurrentStage(nextStage);
        battleManager.SetUpStage(nextStage);
    }

    public void PreviousMap()
    {
        int previousStage = PlayerInfo.Instance.CurrentStage - 1;
        if (previousStage >= 1)
        {
            PlayerInfo.Instance.SetCurrentStage(previousStage);
            battleManager.SetUpStage(previousStage);
        }
        else
        {
            Debug.LogWarning("이미 최소 스테이지에 위치하고 있습니다.");
        }
            
    }
}

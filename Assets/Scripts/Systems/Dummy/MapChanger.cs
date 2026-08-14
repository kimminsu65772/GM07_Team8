using UnityEngine;

public class MapChanger : MonoBehaviour
{
    [SerializeField] private StageTransitionController stageTransitionController;

    public void NextMap()
    {
        int nextStage = PlayerInfo.Instance.CurrentStage + 1;

        if (!stageTransitionController.StartTransition(nextStage))
        {
            return;
        }

        PlayerInfo.Instance.SetCurrentStage(nextStage);
    }

    public void PreviousMap()
    {
        int previousStage = PlayerInfo.Instance.CurrentStage - 1;
        if (previousStage >= 1)
        {
            if (!stageTransitionController.StartTransition(previousStage))
            {
                return;
            }

            PlayerInfo.Instance.SetCurrentStage(previousStage);
        }
        else
        {
            Debug.LogWarning("이미 최소 스테이지에 위치하고 있습니다.");
        }
            
    }
}

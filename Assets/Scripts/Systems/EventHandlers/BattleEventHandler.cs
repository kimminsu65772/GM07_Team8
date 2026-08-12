using UnityEngine;

public class BattleEventHandler : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private StageTestManager stageTestManager;
    [SerializeField] private StageTransitionController stageTransitionController;

    private void OnEnable()
    {
        // 중복 구독 방지
        stageTestManager.OnEnemyKilled -= HandleEnemyKilled;
        stageTestManager.OnStageCompleted -= HandleStageCompleted;
        stageTestManager.OnStageFailed -= HandleStageFailed;

        stageTestManager.OnEnemyKilled += HandleEnemyKilled;
        stageTestManager.OnStageCompleted += HandleStageCompleted;
        stageTestManager.OnStageFailed += HandleStageFailed;
    }

    private void OnDisable()
    {
        stageTestManager.OnEnemyKilled -= HandleEnemyKilled;
        stageTestManager.OnStageCompleted -= HandleStageCompleted;
        stageTestManager.OnStageFailed -= HandleStageFailed;
    }

    private void HandleEnemyKilled()
    {
        // 적이 처치되면 재화 획득 
        // TODO: 스테이지별로 재화 획득량을 다르게 설정할 수 있도록 수정 필요
        PlayerInfo.Instance.AddCurrency(CurrencyType.Gold, 100);
    }

    private void HandleStageCompleted(int clearedStageNumber)
    {
        // 매번 PlayerInfo.Instance를 호출하지 않도록 미리 변수에 할당
        PlayerInfo playerInfo = PlayerInfo.Instance;
        bool isAlreadyCleard = clearedStageNumber <= playerInfo.MaxClearedStage;

        //  첫 클리어시 보상 지급
        if (!isAlreadyCleard)
        {
            int rewardAmount = 100 * clearedStageNumber;
            playerInfo.AddCurrency(CurrencyType.Gold, rewardAmount);
            playerInfo.AddCurrency(CurrencyType.Gems, rewardAmount / 10);
            playerInfo.TryUpdateMaxClearedStage(clearedStageNumber);
        }

        int nextStageNumber;

        if (clearedStageNumber == stageTestManager.LastStage)
        {
            nextStageNumber = stageTestManager.LastStage;
        }
        else if (isAlreadyCleard)
        {
            nextStageNumber = clearedStageNumber;
        }
        else
        {
            nextStageNumber = clearedStageNumber + 1;
        }
        playerInfo.SetCurrentStage(nextStageNumber);
        stageTransitionController.StartTransition(nextStageNumber);
    }

    private void HandleStageFailed(int failedStageNumber, string failedReason)
    {
        // 스테이지 재시작
        // 혹은 이전 스테이지로 넘어가도 될듯
        battleManager.SetUpStage(failedStageNumber);

        // UI한테 실패 이유를 알려주면 해당 메시지를 UI에서 표시하도록?
        // 혹은 enum 타입으로 실패 이유를 정의해서 UI에서 해당 enum에 맞는 메시지를 표시하도록 할 수도 있을듯.

    }
}

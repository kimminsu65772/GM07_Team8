using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageTimeController : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Image stageProgressBar;     // 스테이지 바 이미지 (Filled)
    [SerializeField] private TextMeshProUGUI stageText; // "1-1 Stage" 텍스트
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("진행 설정")]
    [SerializeField] private float fillSpeed = 0.2f;
    private bool isRunning = false;
    private float currentProgress = 0f;

    [Header("보상 및 스테이지 설정")]
    [SerializeField] private CurrencyType rewardCurrencyType = CurrencyType.Gold; // 지급할 재화 종류
    [SerializeField] private int stageClearReward = 100; // 클리어 보상
    [SerializeField] private int targetStageLevel = 1;

    private void Start()
    {
        // 게임 시작 시 현재 골드 UI 반영
        UpdateGoldUI();
    }
    void Update()
    {
        if (isRunning && stageProgressBar != null)
        {
            currentProgress += Time.deltaTime * fillSpeed;
            stageProgressBar.fillAmount = currentProgress;

            // 바가 꽉 찼을 때 (스테이지 클리어)
            if (currentProgress >= 1f)
            {
                currentProgress = 1f;
                isRunning = false;

                // 1. 텍스트 변경 (다음 스테이지 표기 등)
                ChangeStageText("1-2 Stage");

                // 2. StageProgressManager를 통한 최대 클리어 스테이지 갱신 연동
                UpdateStageProgress();

                // 3. WalletManager를 통한 재화 지급 연동
                GiveStageClearReward();
            }
        }
    }
    private void ChangeStageText(string newText)
    {
        if (stageText != null)
        {
            stageText.text = newText;
        }
    }
    private void UpdateStageProgress()
    {
        if (StageProgressManager.Instance != null && StageProgressManager.Instance.IsInitialized)
        {
            // 현재 클리어한 스테이지 번호를 넘겨서 최대 기록 갱신 시도
            bool isNewRecord = StageProgressManager.Instance.TryUpdateMaxClearedStage(targetStageLevel);
            if (isNewRecord)
            {
                Debug.Log($"최고 기록 갱신! 클리어 스테이지: {targetStageLevel}");
            }
            else
            {
                Debug.Log($"스테이지 클리어 완료 (기존 기록보다 낮거나 같음)");
            }
        }
        else
        {
            Debug.LogWarning("StageProgressManager가 존재하지 않거나 초기화되지 않았습니다!");
        }
    }
    private void GiveStageClearReward()
    {
        if (WalletManager.Instance != null && WalletManager.Instance.IsInitialized)
        {
            WalletManager.Instance.TryAdd(rewardCurrencyType, stageClearReward);
            Debug.Log($"스테이지 클리어 보상 지급: {rewardCurrencyType} +{stageClearReward}");
            UpdateGoldUI();
        }
        else
        {
            Debug.LogWarning("WalletManager가 존재하지 않거나 초기화되지 않았습니다!");
        }
    }
    private void UpdateGoldUI()
    {
        if (goldText != null && WalletManager.Instance != null && WalletManager.Instance.IsInitialized)
        {
            int currentGold = WalletManager.Instance.GetAmount(rewardCurrencyType);
            goldText.text = $"{currentGold}";
        }
    }
    // 시작 버튼
    public void OnClickPlay()
    {
        isRunning = true;
        Time.timeScale = 1f; 
    }
    //일시정지 버튼
    public void OnClickPause()
    {
        isRunning = false;
        Time.timeScale = 0f; 
    }
    //감속 버튼 
    public void OnClickFastForward()
    {
        isRunning = true;
        Time.timeScale = 2f; 
    }
    // 리셋 함수
    public void ResetStageBar()
    {
        isRunning = false;
        currentProgress = 0f;
        Time.timeScale = 1f;
        if (stageProgressBar != null) stageProgressBar.fillAmount = 0f;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class StageTimeController : MonoBehaviour
{
    [Header("진행바 연결")]
    [SerializeField] private Image stageProgressBar;

    private bool isRunning = false;
    private float currentProgress = 0f;

    [Header("테스트용 설정")]
    [SerializeField] private float fillSpeed = 0.2f;

    void Update()
    {
        if (isRunning && stageProgressBar != null)
        {
            currentProgress += Time.deltaTime * fillSpeed;
            stageProgressBar.fillAmount = currentProgress;

            if (currentProgress >= 1f)
            {
                currentProgress = 1f;
                isRunning = false;
                Debug.Log("웨이브 완료! 보스 등장 타이밍!");
            }
        }
    }
    // 시작 버튼
    public void OnClickPlay()
    {
        isRunning = true;
        Time.timeScale = 1f; 
        Debug.Log("게임 시작 / 재생");
    }
    //일시정지 버튼
    public void OnClickPause()
    {
        isRunning = false;
        Time.timeScale = 0f; 
        Debug.Log("일시정지");
    }
    //감속 버튼 
    public void OnClickSlowForward()
    {
        isRunning = true;
        Time.timeScale = 0.5f; 
        Debug.Log("2배속 재생");
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

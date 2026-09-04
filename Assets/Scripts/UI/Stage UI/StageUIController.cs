using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageUIController : MonoBehaviour
{
    [SerializeField] TMP_Text stageText;
    [SerializeField] Slider stageProgressSlider;
    [SerializeField] StageManager stageManager;
    [SerializeField] StageCatalog stageCatalog;
    [SerializeField, Min(0f)]
    private float fillDuration = 0.4f;

    private int currentStageNumber;
    private int currentStageInRegion;
    private int currentRegion;
    private int currentWaveInStage;
    private int currentWave;

    private Coroutine fillRoutine;

    private void Awake()
    {
        if (stageText == null) 
        {
            return;
        }

        if (stageProgressSlider == null) 
        {
            return;
        }

        if (stageManager == null) 
        {
            return;
        }   
    }

    private void OnEnable()
    {
        stageManager.OnWaveCompleted -= HandleWaveCompleted;
        stageManager.OnStageStarted -= HandleStageStarted;
        stageManager.OnBossSpawned -= HideStageBar;

        stageManager.OnWaveCompleted += HandleWaveCompleted;
        stageManager.OnStageStarted += HandleStageStarted;
        stageManager.OnBossSpawned += HideStageBar;
    }

    private void OnDisable()
    {
        stageManager.OnWaveCompleted -= HandleWaveCompleted;
        stageManager.OnStageStarted -= HandleStageStarted;
        stageManager.OnBossSpawned -= HideStageBar;

        StopFillAnimation();
    }

    private void Start()
    {
        if (stageManager == null) 
        {
            return;
        }
        currentStageNumber = PlayerInfo.Instance.CurrentStage;
        UpdateStageTextUI(currentStageNumber);

        ShowStageBar();
    }

    private void UpdateStageTextUI(int stageNumber)
    {
        if (stageCatalog.StageCycle <= 0)
        {   
            return;
        }
        currentRegion = ((stageNumber - 1) / stageCatalog.StageCycle) + 1;
        currentStageInRegion = ((stageNumber - 1) % stageCatalog.StageCycle) + 1;
        stageText.text = $"{currentRegion}-{currentStageInRegion} Stage";
    }


    private void HandleWaveCompleted(int waveNumber)
    {
        UpdateStageProgressBar(waveNumber);
    }

    private void StageProgressBarInit()
    {
        UpdateStageProgressBar(0);
        ShowStageBar();
    }

    private void UpdateStageProgressBar(int waveNumber)
    {
        if (currentWaveInStage <= 0)
        {
            if (stageProgressSlider != null) stageProgressSlider.normalizedValue = 0f;
            return;
        }

        currentWave = waveNumber;

        float targetValue = Mathf.Clamp01(
            (float)currentWave / currentWaveInStage);

        StartFillAnimation(targetValue);
    }

    private void StartFillAnimation(float targetValue)
    {
        StopFillAnimation();

        if (fillDuration <= 0f)
        {
            stageProgressSlider.normalizedValue = targetValue;
            return;
        }

        fillRoutine = StartCoroutine(
            AnimateFill(targetValue));
    }

    private IEnumerator AnimateFill(float targetValue)
    {
        float startValue =
            stageProgressSlider.normalizedValue;

        float elapsedTime = 0f;

        while (elapsedTime < fillDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / fillDuration);

            float smoothProgress = Mathf.SmoothStep(
                0f,
                1f,
                progress);

            stageProgressSlider.normalizedValue =
                Mathf.Lerp(
                    startValue,
                    targetValue,
                    smoothProgress);

            yield return null;
        }

        stageProgressSlider.normalizedValue = targetValue;
        fillRoutine = null;
    }

    private void StopFillAnimation()
    {
        if (fillRoutine == null)
            return;

        StopCoroutine(fillRoutine);
        fillRoutine = null;
    }


    private void HandleStageStarted(int stageNumber, int totalWaveCount)
    {
        currentWaveInStage = totalWaveCount;
        currentStageNumber = stageNumber;
        UpdateStageTextUI(currentStageNumber);
        StageProgressBarInit();
    }
    public void HideStageBar()
    {
        if (stageText != null) stageText.gameObject.SetActive(false);
        if (stageProgressSlider != null) stageProgressSlider.gameObject.SetActive(false);
    }

    public void ShowStageBar()
    {
        if (stageText != null) stageText.gameObject.SetActive(true);
        if (stageProgressSlider != null) stageProgressSlider.gameObject.SetActive(true);
    }
}

using UnityEngine;
using UnityEngine.UI;
public class StageProgressSlider : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider progressSlider;

    [Header("Data Reference")]
    [SerializeField] private StageCatalog stageCatalog;

    private void Start()
    {
        if (progressSlider == null)
        {
            progressSlider = GetComponent<Slider>();
        }
        UpdateSlider();
    }

    public void UpdateSlider()
    {
        if (stageCatalog == null || stageCatalog.StageCount <= 0)
        { 
            return;
        }

        if (PlayerInfo.Instance == null || !PlayerInfo.Instance.IsInitialized)
        {
            if (progressSlider != null) progressSlider.value = 0f;
            return;
        }

        int totalStages = stageCatalog.StageCount;
        int currentProgress = PlayerInfo.Instance.MaxClearedStage; 

        float fillAmount = Mathf.Clamp01((float)currentProgress / totalStages);

        if (progressSlider != null)
        {
            progressSlider.value = fillAmount;
        }
    }
}
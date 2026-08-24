using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class StageMapSlot : MonoBehaviour
{
    [SerializeField] private int stageNumber;
    [SerializeField] private Button slotButton;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private GameObject lockObject; 

    private StageMapUI mapUI;

    public void Init(StageMapUI worldMapUI, int stageNum, int maxClearedStage)
    {
        mapUI = worldMapUI;
        stageNumber = stageNum;

        if (stageText != null)
        {
            stageText.text = stageNumber.ToString();
        }

        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => mapUI.OnStageSelected(stageNumber));
        }
        RefreshState(maxClearedStage);
    }
    public void RefreshState(int maxClearedStage)
    {
        bool isUnlocked = stageNumber <= maxClearedStage + 1;

        if (slotButton != null)
        {
            slotButton.interactable = isUnlocked;
        }

        if (lockObject != null)
        {
            lockObject.SetActive(!isUnlocked);
        }
    }
    public void Refresh(int maxClearedStage)
    {
        RefreshState(maxClearedStage);
    }
}

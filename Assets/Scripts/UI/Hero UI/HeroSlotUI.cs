using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class HeroSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI nameLevelText;
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private Button slotButton; 

    private HeroEntry currentEntry;
    private HeroSaveData currentSaveData;
    private System.Action<HeroEntry, HeroSaveData> onClickCallback;

    public void SetupSlot(HeroEntry entry, HeroSaveData saveInfo, bool isOwned, System.Action<HeroEntry, HeroSaveData> onClick)
    {
        currentEntry = entry;
        currentSaveData = saveInfo;
        onClickCallback = onClick;

        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => {
                onClickCallback?.Invoke(currentEntry, currentSaveData);
            });
        }

        if (entry == null || !isOwned || saveInfo == null)
        {
            if (nameLevelText != null) nameLevelText.text = "잠김";
            if (statText != null) statText.text = "";
            if (lockOverlay != null) lockOverlay.SetActive(true);
            if (slotButton != null) slotButton.interactable = false; 
            return;
        }

        int level = saveInfo.Level;

        if (nameLevelText != null)
        {
            nameLevelText.text = $"{entry.HeroName} LV.{level}";
        }

        if (statText != null)
        {
            statText.text = $"위치: {entry.HeroLocation}";
        }

        if (lockOverlay != null)
        {
            lockOverlay.SetActive(false);
        }

        if (slotButton != null)
        {
            slotButton.interactable = true;
        }
    }
}
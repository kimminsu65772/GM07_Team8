using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CraftUI : MonoBehaviour
{
    [SerializeField] private Image craftIcon;
    [SerializeField] private TMP_Text durationText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Button craftButton;

    private bool isFurnaceSoundPlaying;
    private EquipmentCraftRecipeDB recipeDB;
    private Action onCraftCompleted;
    private Coroutine refreshCo;
    private WaitForSeconds seconds1 = new WaitForSeconds(1f);

    private void Awake()
    {
        if (craftButton != null)
        {
            craftButton.onClick.RemoveListener(OnCraftButtonClicked);
            craftButton.onClick.AddListener(OnCraftButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (craftButton != null)
        {
            craftButton.onClick.RemoveListener(OnCraftButtonClicked);
        }
    }

    private void OnEnable()
    {
        RefreshView();
        StartRefreshCo();
    }

    private void OnDisable()
    {
        StopRefreshCo();
    }

    public void Bind(EquipmentCraftRecipeDB recipeDB, Action onCraftCompleted)
    {
        this.recipeDB = recipeDB;
        this.onCraftCompleted = onCraftCompleted;

        RefreshView();

        if (isActiveAndEnabled)
        {
            StartRefreshCo();
        }
    }
    private void StartRefreshCo()
    {
        StopRefreshCo();

        EquipmentCraftSlotSaveData craftSlot =
            PlayerInfo.Instance.GetEquipmentCraftSlot(0);

        // 제작 슬롯이 없거나 제작 상태가 아니라면 코루틴을 시작하지 않는다.
        if (craftSlot == null || !craftSlot.IsCrafting)
        {
            return;
        }

        refreshCo = StartCoroutine(RefreshProgressCo());
    }

    private IEnumerator RefreshProgressCo()
    {
        while (true)
        {
            RefreshView();

            EquipmentCraftSlotSaveData craftSlot =
                PlayerInfo.Instance.GetEquipmentCraftSlot(0);

            if (craftSlot == null || !craftSlot.IsCrafting)
            {
                yield break;
            }

            if (PlayerInfo.Instance.IsEquipmentCraftComplete(0, DateTime.UtcNow))
            {
                yield break;
            }

            yield return seconds1;
        }
    }
    private void StopRefreshCo()
    {
        if (refreshCo != null)
        {
            StopCoroutine(refreshCo);
            refreshCo = null;
        }
    }

    private void RefreshView()
    {
        EquipmentCraftSlotSaveData craftSlot = PlayerInfo.Instance.GetEquipmentCraftSlot(0);
        if (craftSlot == null || !craftSlot.IsCrafting)
        {
            Clear();
            return;
        }

        bool isCrafting = craftSlot.IsCrafting;

        if (isCrafting && !isFurnaceSoundPlaying)
        {
            isFurnaceSoundPlaying = true;
            SoundManager.Instance.PlaySound(SoundId.Crafting);
        }
        else if (!isCrafting && isFurnaceSoundPlaying)
        {
            isFurnaceSoundPlaying = false;
        }

        EquipmentCraftRecipeSO recipe = recipeDB != null
            ? recipeDB.GetRecipeById(craftSlot.RecipeId)
            : null;

        if (recipe == null)
        {
            Clear();
            return;
        }

        DateTime nowUtc = DateTime.UtcNow;
        bool isComplete = PlayerInfo.Instance.IsEquipmentCraftComplete(craftSlot.SlotIndex, nowUtc);

        if (craftIcon != null)
        {
            craftIcon.enabled = craftIcon.sprite != null;
        }

        if (durationText != null)
        {
            durationText.text = isComplete
                ? "완료"
                : FormatRemainingDuration(craftSlot.CompletesAtUtc, nowUtc);
        }

        if (progressSlider != null)
        {
            progressSlider.value = CalculateProgress(craftSlot, nowUtc);
        }

        if (craftButton != null)
        {
            craftButton.interactable = isComplete;
        }
    }

    private void Clear()
    {
        if (craftIcon != null)
        {
            craftIcon.enabled = false;
        }

        if (durationText != null)
            durationText.text = string.Empty;

        if (progressSlider != null)
            progressSlider.value = 0f;

        if (craftButton != null)
            craftButton.interactable = false;
    }

    private void OnCraftButtonClicked()
    {
        if (!PlayerInfo.Instance.IsEquipmentCraftComplete(0, DateTime.UtcNow))
        {
            return;
        }

        onCraftCompleted?.Invoke();
    }

    private static float CalculateProgress(EquipmentCraftSlotSaveData craftSlot, DateTime nowUtc)
    {
        if (!TryParseCraftTimes(craftSlot, out DateTime startedAtUtc, out DateTime completesAtUtc))
        {
            return 0f;
        }

        double totalSeconds = (completesAtUtc - startedAtUtc).TotalSeconds;
        if (totalSeconds <= 0)
        {
            return 1f;
        }

        double elapsedSeconds = (nowUtc - startedAtUtc).TotalSeconds;
        return Mathf.Clamp01((float)(elapsedSeconds / totalSeconds));
    }

    private static string FormatRemainingDuration(string completesAtUtcText, DateTime nowUtc)
    {
        if (!DateTime.TryParse(completesAtUtcText, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime completesAtUtc))
        {
            return string.Empty;
        }

        int remainingSeconds = Mathf.Max(0, Mathf.CeilToInt((float)(completesAtUtc - nowUtc).TotalSeconds));
        return FormatDuration(remainingSeconds);
    }

    private static bool TryParseCraftTimes(EquipmentCraftSlotSaveData craftSlot, out DateTime startedAtUtc, out DateTime completesAtUtc)
    {
        startedAtUtc = default;
        completesAtUtc = default;

        if (craftSlot == null)
        {
            return false;
        }

        return DateTime.TryParse(craftSlot.StartedAtUtc, null, System.Globalization.DateTimeStyles.RoundtripKind, out startedAtUtc)
            && DateTime.TryParse(craftSlot.CompletesAtUtc, null, System.Globalization.DateTimeStyles.RoundtripKind, out completesAtUtc);
    }

    private static string FormatDuration(int seconds)
    {
        if (seconds <= 0)
            return "즉시";

        TimeSpan duration = TimeSpan.FromSeconds(seconds);

        if (duration.TotalHours >= 1)
            return $"{(int)duration.TotalHours}시간 {duration.Minutes}분 {duration.Seconds}초";

        if (duration.TotalMinutes >= 1)
            return $"{duration.Minutes}분 {duration.Seconds}초";

        return $"{duration.Seconds}초";
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroFormationTestUI : MonoBehaviour
{
    [Header("Owned Heroes")]
    [SerializeField] private Transform heroButtonRoot;
    [SerializeField] private Button heroButtonTemplate;

    [Header("Formation Slots")]
    [SerializeField] private Transform slotButtonRoot;
    [SerializeField] private Button slotButtonTemplate;

    [Header("Status")]
    [SerializeField] private TMP_Text selectedHeroText;
    [SerializeField] private TMP_Text resultText;

    [Header("Optional Battle Refresh")]
    [SerializeField] private BattleManager battleManager;

    private readonly List<Button> generatedHeroButtons = new();
    private readonly List<Button> generatedSlotButtons = new();
    private string selectedHeroName;

    private void OnEnable()
    {
        RefreshView();
    }

    private void RefreshView()
    {
        BuildOwnedHeroButtons();
        BuildSlotButtons();
        RefreshSelectionView();
    }

    private void BuildOwnedHeroButtons()
    {
        ClearButtons(generatedHeroButtons);

        if (heroButtonRoot == null || heroButtonTemplate == null)
        {
            SetResult("Hero button root or template is not assigned.");
            return;
        }

        Dictionary<string, HeroSaveData> heroes = PlayerInfo.Instance.Heroes;
        if (heroes == null)
        {
            return;
        }

        List<string> ownedHeroNames = new();
        foreach (KeyValuePair<string, HeroSaveData> pair in heroes)
        {
            if (pair.Value != null && pair.Value.IsOwned)
            {
                ownedHeroNames.Add(pair.Key);
            }
        }

        ownedHeroNames.Sort();

        foreach (string heroName in ownedHeroNames)
        {
            Button button = Instantiate(heroButtonTemplate, heroButtonRoot);
            button.gameObject.SetActive(true);
            button.onClick.AddListener(() => ToggleHeroSelection(heroName));
            SetButtonLabel(button, GetHeroButtonLabel(heroName));
            generatedHeroButtons.Add(button);
        }
    }

    private void BuildSlotButtons()
    {
        ClearButtons(generatedSlotButtons);

        if (slotButtonRoot == null || slotButtonTemplate == null)
        {
            SetResult("Slot button root or template is not assigned.");
            return;
        }

        HeroFormationSaveData formation = PlayerInfo.Instance.HeroFormation;
        if (formation == null || formation.Slots == null)
        {
            return;
        }

        List<HeroSaveSlot> slots = new();
        foreach (HeroSaveSlot slot in formation.Slots)
        {
            if (slot != null)
            {
                slots.Add(slot);
            }
        }

        slots.Sort((left, right) =>
            left.SlotIndex.CompareTo(right.SlotIndex));

        foreach (HeroSaveSlot slot in slots)
        {
            int slotIndex = slot.SlotIndex;
            Button button = Instantiate(slotButtonTemplate, slotButtonRoot);
            button.gameObject.SetActive(true);
            button.onClick.AddListener(() => HandleSlotClicked(slotIndex));
            SetButtonLabel(button, GetSlotButtonLabel(slot));
            generatedSlotButtons.Add(button);
        }
    }

    private void ToggleHeroSelection(string heroName)
    {
        selectedHeroName = selectedHeroName == heroName
            ? null
            : heroName;

        RefreshView();
    }

    private void HandleSlotClicked(int slotIndex)
    {
        HeroFormationManager formationManager =
            HeroFormationManager.Instance;

        if (formationManager == null)
        {
            SetResult("HeroFormationManager was not found.");
            return;
        }

        bool success;

        if (!string.IsNullOrWhiteSpace(selectedHeroName))
        {
            success = formationManager.TrySetHeroToSlot(
                slotIndex,
                selectedHeroName);
        }
        else if (TryGetSlot(slotIndex, out HeroSaveSlot slot) &&
                 !string.IsNullOrWhiteSpace(slot.HeroName))
        {
            success = formationManager.ClearSlot(slotIndex);
        }
        else
        {
            return;
        }

        SetResult(success
            ? $"Slot {slotIndex} updated."
            : $"Slot {slotIndex} update failed.");

        if (success && battleManager != null)
        {
            battleManager.SetUpStage(PlayerInfo.Instance.CurrentStage);
        }

        RefreshView();
    }

    private bool TryGetSlot(int slotIndex, out HeroSaveSlot result)
    {
        result = null;

        HeroFormationSaveData formation = PlayerInfo.Instance.HeroFormation;
        if (formation == null || formation.Slots == null)
        {
            return false;
        }

        foreach (HeroSaveSlot slot in formation.Slots)
        {
            if (slot != null && slot.SlotIndex == slotIndex)
            {
                result = slot;
                return true;
            }
        }

        return false;
    }

    private string GetHeroButtonLabel(string heroName)
    {
        string selectionPrefix = selectedHeroName == heroName
            ? "> "
            : string.Empty;

        return HeroFormationManager.Instance != null &&
               HeroFormationManager.Instance.IsHeroInFormation(heroName)
            ? $"{selectionPrefix}{heroName} (Placed)"
            : $"{selectionPrefix}{heroName}";
    }

    private string GetSlotButtonLabel(HeroSaveSlot slot)
    {
        string heroName = string.IsNullOrWhiteSpace(slot.HeroName)
            ? "Empty"
            : slot.HeroName;

        return $"Slot {slot.SlotIndex}: {heroName}";
    }

    private void RefreshSelectionView()
    {
        if (selectedHeroText != null)
        {
            selectedHeroText.text =
                string.IsNullOrWhiteSpace(selectedHeroName)
                    ? string.Empty
                    : $"Selected: {selectedHeroName}";
        }
    }

    private void ClearButtons(List<Button> buttons)
    {
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        buttons.Clear();
    }

    private void SetButtonLabel(Button button, string label)
    {
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>(true);
        if (buttonText != null)
        {
            buttonText.text = label;
        }
    }

    private void SetResult(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
        }
    }
}

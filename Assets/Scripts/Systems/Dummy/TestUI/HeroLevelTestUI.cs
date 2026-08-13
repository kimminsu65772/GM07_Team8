using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroLevelTestUI : MonoBehaviour
{
    private const int MaxPlacementPointCount = 5;
    private const int TestMaxHeroLevel = 3;

    [Header("Hero List")]
    [SerializeField] private Transform heroButtonRoot;
    [SerializeField] private Button heroButtonTemplate;

    [Header("Selection")]
    [SerializeField] private Button levelUpButton;
    [SerializeField] private TMP_Text selectedHeroText;
    [SerializeField] private TMP_Text resultText;

    [Header("Optional Runtime Sync")]
    [SerializeField] private AirshipHeroPlacementPoints placementPoints;

    private readonly List<Button> generatedHeroButtons = new();
    private string selectedHeroName;

    private void OnEnable()
    {
        if (levelUpButton != null)
        {
            levelUpButton.onClick.AddListener(LevelUpSelectedHero);
            levelUpButton.gameObject.SetActive(false);
        }

        BuildOwnedHeroButtons();
        RefreshSelectionView();
    }

    private void OnDisable()
    {
        if (levelUpButton != null)
        {
            levelUpButton.onClick.RemoveListener(LevelUpSelectedHero);
        }
    }

    private void BuildOwnedHeroButtons()
    {
        ClearGeneratedButtons();

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
            button.onClick.AddListener(() => SelectHero(heroName));

            SetButtonLabel(button, GetHeroButtonLabel(heroName));
            generatedHeroButtons.Add(button);
        }
    }

    private void SelectHero(string heroName)
    {
        selectedHeroName = heroName;
        RefreshSelectionView();
    }

    private void LevelUpSelectedHero()
    {
        if (string.IsNullOrWhiteSpace(selectedHeroName))
        {
            return;
        }

        PlayerInfo playerInfo = PlayerInfo.Instance;
        if (!playerInfo.TryGetHeroData(
                selectedHeroName,
                out HeroSaveData heroData))
        {
            SetResult("Selected hero data was not found.");
            return;
        }

        if (heroData.Level >= TestMaxHeroLevel)
        {
            SetResult($"{selectedHeroName} is already at max level.");
            return;
        }

        int nextLevel = heroData.Level + 1;
        if (!playerInfo.SetHeroLevel(selectedHeroName, nextLevel))
        {
            SetResult($"{selectedHeroName} level up failed.");
            return;
        }

        if (TryGetPlacedHero(selectedHeroName, out Hero placedHero))
        {
            HeroLvManager.Instance.LvSet(nextLevel, placedHero);
        }

        SetResult($"{selectedHeroName} reached Lv.{nextLevel}.");
        BuildOwnedHeroButtons();
        RefreshSelectionView();
    }

    private bool TryGetPlacedHero(string heroName, out Hero result)
    {
        result = null;

        if (placementPoints == null)
        {
            return false;
        }

        if (TryGetPlacedHero(
                placementPoints.GetPlacementTransforms(
                    MaxPlacementPointCount,
                    true),
                heroName,
                out result))
        {
            return true;
        }

        return TryGetPlacedHero(
            placementPoints.GetPlacementTransforms(
                MaxPlacementPointCount,
                false),
            heroName,
            out result);
    }

    private bool TryGetPlacedHero(
        Transform[] points,
        string heroName,
        out Hero result)
    {
        result = null;

        if (points == null)
        {
            return false;
        }

        foreach (Transform point in points)
        {
            if (point == null)
            {
                continue;
            }

            for (int i = 0; i < point.childCount; i++)
            {
                Hero hero = point.GetChild(i).GetComponent<Hero>();
                if (hero != null && hero.HeroName == heroName)
                {
                    result = hero;
                    return true;
                }
            }
        }

        return false;
    }

    private string GetHeroButtonLabel(string heroName)
    {
        return PlayerInfo.Instance.TryGetHeroData(
            heroName,
            out HeroSaveData heroData)
                ? $"{heroName} Lv.{heroData.Level}"
                : heroName;
    }

    private void RefreshSelectionView()
    {
        bool hasSelection =
            !string.IsNullOrWhiteSpace(selectedHeroName);

        if (selectedHeroText != null)
        {
            selectedHeroText.text = hasSelection
                ? $"Selected: {GetHeroButtonLabel(selectedHeroName)}"
                : string.Empty;
        }

        if (levelUpButton != null)
        {
            levelUpButton.gameObject.SetActive(hasSelection);
        }
    }

    private void ClearGeneratedButtons()
    {
        foreach (Button button in generatedHeroButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        generatedHeroButtons.Clear();
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

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoHeroStatGUI : MonoBehaviour
{
    private const int MaxPlacementPointCount = 5;

    [SerializeField] private AirshipHeroPlacementPoints placementPoints;
    [SerializeField] private TMP_Text[] heroStatTexts;
    [SerializeField] private TMP_Text airshipStatText;

    private readonly List<Transform> cachedPlacementPoints = new();
    private readonly List<Hero> placedHeroes = new();

    private AirshipStatController airshipStatController;
    private AirshipHealth airshipHealth;
    private AirshipMovement airshipMovement;
    private AirshipUpgradeController airshipUpgradeController;

    private void Awake()
    {
        CachePlacementPoints();
        CacheAirshipComponents();
        ClearTexts();
    }

    private void LateUpdate()
    {
        UpdateAirshipStatText();
        UpdateHeroStatTexts();
    }

    private void UpdateHeroStatTexts()
    {
        if (placementPoints == null || heroStatTexts == null)
        {
            return;
        }

        CollectPlacedHeroes();

        if (placedHeroes.Count == 0)
        {
            ClearHeroTexts();
            return;
        }

        for (int i = 0; i < heroStatTexts.Length; i++)
        {
            TMP_Text statText = heroStatTexts[i];
            if (statText == null)
            {
                continue;
            }

            statText.text = i < placedHeroes.Count
                ? GetHeroStatText(placedHeroes[i])
                : string.Empty;
        }
    }

    private void CacheAirshipComponents()
    {
        if (placementPoints == null)
        {
            return;
        }

        airshipStatController =
            placementPoints.GetComponent<AirshipStatController>();
        airshipHealth =
            placementPoints.GetComponent<AirshipHealth>();
        airshipMovement =
            placementPoints.GetComponent<AirshipMovement>();
        airshipUpgradeController =
            placementPoints.GetComponent<AirshipUpgradeController>();
    }

    private void CachePlacementPoints()
    {
        cachedPlacementPoints.Clear();

        if (placementPoints == null)
        {
            return;
        }

        AddPlacementPoints(
            placementPoints.GetPlacementTransforms(MaxPlacementPointCount, true));
        AddPlacementPoints(
            placementPoints.GetPlacementTransforms(MaxPlacementPointCount, false));
    }

    private void AddPlacementPoints(Transform[] points)
    {
        if (points == null)
        {
            return;
        }

        foreach (Transform point in points)
        {
            if (point != null && !cachedPlacementPoints.Contains(point))
            {
                cachedPlacementPoints.Add(point);
            }
        }
    }

    private void CollectPlacedHeroes()
    {
        placedHeroes.Clear();

        foreach (Transform placementPoint in cachedPlacementPoints)
        {
            if (placementPoint == null)
            {
                continue;
            }

            for (int i = 0; i < placementPoint.childCount; i++)
            {
                Hero hero = placementPoint.GetChild(i).GetComponent<Hero>();
                if (hero != null && !placedHeroes.Contains(hero))
                {
                    placedHeroes.Add(hero);
                }
            }
        }
    }

    private string GetHeroStatText(Hero hero)
    {
        return $"id : {hero.HeroID}\n" +
               $"name : {hero.HeroName}\n" +
               $"level : {hero.HeroLv}\n" +
               $"hp : {hero.HeroCurrentHP} / {hero.HeroMaxHP}\n" +
               $"atk : {hero.HeroAtk}\n" +
               $"def : {hero.HeroDef}\n" +
               $"atktime : {hero.HeroAttackTime}\n" +
               $"state : {hero.HeroState}";
    }

    private void UpdateAirshipStatText()
    {
        if (airshipStatText == null)
        {
            return;
        }

        if (airshipStatController == null)
        {
            airshipStatText.text = string.Empty;
            return;
        }

        AirshipRuntimeStats stats = airshipStatController.CurrentStats;
        if (stats == null)
        {
            airshipStatText.text = string.Empty;
            return;
        }

        float currentHealth = airshipHealth != null
            ? airshipHealth.CurrentHealth
            : 0f;
        float maxHealth = airshipHealth != null
            ? airshipHealth.MaxHealth
            : stats.MaxHealth;
        float currentMoveSpeed = airshipMovement != null
            ? airshipMovement.CurrentMoveSpeed
            : 0f;

        string levelText = GetAirshipLevelText();

        airshipStatText.text =
            $"airship\n" +
            levelText +
            $"hp : {currentHealth:F1} / {maxHealth:F1}\n" +
            $"atk : {stats.Attack:F1}\n" +
            $"def : {stats.Recovery:F1}\n" +
            $"critical : {stats.CriticalChance * 100f:F1}%\n" +
            $"move speed : {currentMoveSpeed:F1} / {stats.MoveSpeed:F1}\n" +
            $"attack speed : {stats.AttackSpeed:F1}";
    }

    private string GetAirshipLevelText()
    {
        if (airshipUpgradeController == null)
        {
            return string.Empty;
        }

        AirshipUpgradeState state =
            airshipUpgradeController.UpgradeState;

        return $"level : " +
               $"atk {state.AttackLevel}, " +
               $"def {state.RecoveryLevel}, " +
               $"hp {state.MaxHealthLevel}, " +
               $"critical {state.CriticalLevel}\n";
    }

    private void ClearTexts()
    {
        if (airshipStatText != null)
        {
            airshipStatText.text = string.Empty;
        }

        ClearHeroTexts();
    }

    private void ClearHeroTexts()
    {
        if (heroStatTexts == null)
        {
            return;
        }

        foreach (TMP_Text statText in heroStatTexts)
        {
            if (statText != null)
            {
                statText.text = string.Empty;
            }
        }
    }
}

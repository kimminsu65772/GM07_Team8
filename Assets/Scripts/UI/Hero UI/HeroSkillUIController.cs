using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroSkillUIController : MonoBehaviour
{
    [Header("영웅 배치")]
    [SerializeField] private AirshipHeroPlacementPoints placementPoints;

    [Header("영웅 스킬 버튼")]
    [SerializeField] private HeroSkillButtonUI[] skillButtons;

    [Header("전체 자동 / 수동")]
    [SerializeField] private Button autoSkillButton;
    [SerializeField] private TMP_Text autoSkillButtonText;

    private readonly List<Hero> placedHeroes = new();

    private const int MaxHeroCount = 5;

    private bool isAutoSkill = true;

    private Coroutine refreshCoroutine;

    private float refreshTimer;
    private const float RefreshInterval = 0.2f;

    private void Awake()
    {
        if (autoSkillButton != null)
        {
            autoSkillButton.onClick.AddListener(OnClickAutoSkill);
        }
        UpdateAutoSkillButtonText();
    }
    private void OnEnable()
    {
        refreshCoroutine = StartCoroutine(WaitForHeroesAndRefresh());
    }
    private void OnDisable()
    {
        if (refreshCoroutine != null)
        {
            StopCoroutine(refreshCoroutine);
            refreshCoroutine = null;
        }
    }
    private void Update()
    {
        refreshTimer += Time.unscaledDeltaTime;

        if (refreshTimer >= RefreshInterval)
        {
            refreshTimer = 0f;
            RefreshSkillButtons();
        }
    }
    private IEnumerator WaitForHeroesAndRefresh()
    {
        float timer = 0f;

        while (timer < 3f)
        {
            Hero[] heroes = FindObjectsByType<Hero>(FindObjectsInactive.Exclude, FindObjectsSortMode.None );

            if (heroes.Length > 0)
            {
                yield return null;

                RefreshSkillButtons();
                refreshCoroutine = null;

                yield break;
            }
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        refreshCoroutine = null;
    }
    public void RefreshSkillButtons()
    {
        CollectPlacedHeroes();

        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (skillButtons[i] == null) continue;
            if (i < placedHeroes.Count)
            {
                Hero hero = placedHeroes[i];
                skillButtons[i].SetHero(hero);
            }
            else
            {
                skillButtons[i].ClearHero();
            }
        }
        ApplyAutoSkillMode();
    }
    /// 전열 + 후열 영웅 찾기
    private void CollectPlacedHeroes()
    {
        placedHeroes.Clear();
        Hero[] allHeroes = FindObjectsByType<Hero>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);
        foreach (Hero hero in allHeroes)
        {
            if (hero == null) continue;
            if (placedHeroes.Contains(hero)) continue;
            // 전열
            if (hero.Location == HeroLocationEnum.Front)
            {
                placedHeroes.Add(hero);
                continue;
            }
            // 후열
            if (hero.Location == HeroLocationEnum.Back)
            {
                placedHeroes.Add(hero);
            }
        }
        if (placedHeroes.Count > MaxHeroCount)
        {
            placedHeroes.RemoveRange(
                MaxHeroCount,
                placedHeroes.Count - MaxHeroCount
            );
        }
    }
    private void OnClickAutoSkill()
    {
        isAutoSkill = !isAutoSkill;

        ApplyAutoSkillMode();
        UpdateAutoSkillButtonText();
    }
    private void ApplyAutoSkillMode()
    {
        foreach (Hero hero in placedHeroes)
        {
            if (hero == null) continue;

            HeroAttack heroAttack = hero.GetComponent<HeroAttack>();

            if (heroAttack == null) continue;

            heroAttack.SetAutoSkill(isAutoSkill);
        }
    }
    private void UpdateAutoSkillButtonText()
    {
        if (autoSkillButtonText == null) return;
        autoSkillButtonText.text = isAutoSkill ? "자동" : "수동";
    }
    public void Refresh()
    {
        RefreshSkillButtons();
    }
}
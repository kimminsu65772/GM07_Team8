using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroSkillUIController : MonoBehaviour
{
    [Header("영웅 스킬 버튼")]
    [SerializeField] private HeroSkillButtonUI[] skillButtons;

    [Header("전체 자동 / 수동")]
    [SerializeField] private Button autoSkillButton;
    [SerializeField] private GameObject autoGlowObject;
    [SerializeField] private RectTransform autoGlowTransform;
    [SerializeField] private float rotationSpeed = 200f;

    [SerializeField] private HeroCatalog heroCatalog;
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
        UpdateAutoSkillGlow();
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
        if (isAutoSkill && autoGlowTransform != null)
        {
            autoGlowTransform.Rotate(0f, 0f, -rotationSpeed * Time.unscaledDeltaTime);
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
                if (heroCatalog != null && heroCatalog.TryGetHeroEntry((HeroNameEnum)hero.HeroID, out HeroEntry entry))
                {
                    if (entry.SkillIcon != null)
                    {
                        skillButtons[i].SetSkillIcon(entry.SkillIcon);
                    }
                }
            }
            else
            {
                skillButtons[i].ClearHero();
            }
        }

        bool hasHeroes = placedHeroes.Count > 0;

        if (autoSkillButton != null)
        {
            autoSkillButton.gameObject.SetActive(hasHeroes);
        }
        if (autoGlowObject != null)
        {
            autoGlowObject.SetActive(hasHeroes && isAutoSkill);
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
        UpdateAutoSkillGlow();
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
    private void UpdateAutoSkillGlow()
    {
        if (autoGlowObject != null)
        {
            autoGlowObject.SetActive(isAutoSkill);
        }
    }
    public void Refresh()
    {
        RefreshSkillButtons();
    }
}
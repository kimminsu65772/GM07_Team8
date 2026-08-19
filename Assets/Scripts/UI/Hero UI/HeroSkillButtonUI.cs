using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroSkillButtonUI : MonoBehaviour
{
    [Header("스킬 버튼")]
    [SerializeField]
    private Button skillButton;

    [Header("쿨타임 UI")]
    [SerializeField]
    private Image cooldownFill;

    [SerializeField]
    private TMP_Text cooldownText;

    private Hero hero;
    private HeroAttack heroAttack;

    private void Awake()
    {
        if (skillButton == null)
        {
            skillButton = GetComponent<Button>();
        }
        if (skillButton != null)
        {
            skillButton.onClick.AddListener(OnClickSkill);
        }
    }
    private void Update()
    {
        UpdateCooldownUI();
        UpdateButtonState();
    }
    public void SetHero(Hero targetHero)
    {
        hero = targetHero;

        if (hero != null)
        {
            heroAttack = hero.GetComponent<HeroAttack>();
        }
        else
        {
            heroAttack = null;
        }
        UpdateButtonState();
    }
    public void ClearHero()
    {
        hero = null;
        heroAttack = null;

        if (cooldownFill != null)
            cooldownFill.fillAmount = 0f;

        if (cooldownText != null)
            cooldownText.text = "";

        if (skillButton != null)
            skillButton.interactable = false;
    }
    private void OnClickSkill()
    {
        if (hero == null) return;
        if (heroAttack == null) return;
        if (hero.IsDead) return;
        if (heroAttack.IsAutoSkill) return;
        // 쿨타임 확인
        if (heroAttack.SkillTimer < hero.HeroSkillTime) return;

        hero.SearchEnemy();
        if (hero.TargetEnemy == null) return;
        heroAttack.UseSkill(hero.TargetEnemy);
    }

    private void UpdateButtonState()
    {
        if (skillButton == null) return;

        bool canUse = hero != null && heroAttack != null && !hero.IsDead && !heroAttack.IsAutoSkill
            &&heroAttack.SkillTimer >= hero.HeroSkillTime;
        skillButton.interactable = canUse;
    }

    private void UpdateCooldownUI()
    {
        if (hero == null || heroAttack == null) return;
        float maxTime = hero.HeroSkillTime;

        if (maxTime <= 0f) return;
        float currentTime = heroAttack.SkillTimer;

        float remaining = Mathf.Max( maxTime - currentTime,0f);

        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = remaining / maxTime;
        }

        if (cooldownText != null)
        {
            cooldownText.text = remaining > 0f ? remaining.ToString("F1") : "";
        }
    }
}
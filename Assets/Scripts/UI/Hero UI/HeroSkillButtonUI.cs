using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class HeroSkillButtonUI : MonoBehaviour
{
    [Header("스킬 버튼")]
    [SerializeField] private Button skillButton;

    [Header("쿨타임 UI")]
    [SerializeField] private Image cooldownFill;

    [SerializeField] private TMP_Text cooldownText;

    [SerializeField] private Image skillIcon;

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
            gameObject.SetActive(true);
        }
        else
        {
            heroAttack = null;
            SetSkillIcon(null);
            gameObject.SetActive(false);
        }
        UpdateButtonState();
    }
    public void SetSkillIcon(Sprite icon)
    {
        if (skillIcon == null) return;

        skillIcon.sprite = icon;
        skillIcon.gameObject.SetActive(icon != null);
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

        gameObject.SetActive(false);
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
        if (hero.IsDead) return;
        heroAttack.UseSkill(hero.TargetEnemy);
    }

    private void UpdateButtonState()
    {
        if (skillButton == null)return;
        bool canUse = false;

        if (hero != null && heroAttack != null && hero.gameObject.activeInHierarchy && !hero.IsDead && !heroAttack.IsAutoSkill && heroAttack.SkillTimer >= hero.HeroSkillTime)
        {
            canUse = true;
        }
        skillButton.interactable = canUse;
    }

    private void UpdateCooldownUI()
    {
        if (hero == null || heroAttack == null)
        {
            if (cooldownFill != null)
            { 
                cooldownFill.fillAmount = 0f;
            }
            if (cooldownText != null)
            {
                cooldownText.text = "";
            }
            return;
        }
        float maxTime = hero.HeroSkillTime;

        if (maxTime <= 0f)
        {
            if (cooldownFill != null)
            {
                cooldownFill.fillAmount = 0f;
            }
            if (cooldownText != null)
            {
                cooldownText.text = "";
            }
            return;
        }
        if (hero.IsDead)
        {
            if (cooldownFill != null)
            {
                cooldownFill.fillAmount = 0f;
            }

            if (cooldownText != null)
            {
                cooldownText.text = "";
            }
            return;
        }
        float currentTime = heroAttack.SkillTimer;

        float remaining = Mathf.Max(maxTime - currentTime,0f);
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
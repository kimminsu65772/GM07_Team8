using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroArrangeStatUI : MonoBehaviour
{
    [SerializeField] private HeroStatUI[] heroStatUIs;
    [SerializeField] private TMP_Text heroFormationText;
    [SerializeField] private TMP_Text heroNameText;
    [SerializeField] private Image skillIcon;

    [Header("스킬 툴팁 데이터 연동")]
    [SerializeField] private Button skillIconButton;     // 스킬 아이콘 버튼
    [SerializeField] private SkillToolTipUI skillToolTipUI; // 데이터 셋팅을 위한 툴팁 컴포넌트

    private HeroEntry currentHeroEntry;

    private void Awake()
    {
        // 스킬 아이콘 버튼 클릭 시 툴팁 데이터 갱신 메서드 연결
        if (skillIconButton != null)
        {
            skillIconButton.onClick.AddListener(OnClickSkillIcon);
        }
    }
    public void SetHeroStatUIs(HeroEntry heroEntry)
    {
        ClearHeroStatUIs();
        if (heroEntry == null)
        {
            return;
        }

        if (heroStatUIs == null)
        {
            return;
        }

        if (PlayerInfo.Instance == null ||
            !PlayerInfo.Instance.TryGetHeroData(heroEntry.HeroId, out HeroSaveData heroData))
        {
            return;
        }

        currentHeroEntry = heroEntry;

        HeroStat heroStat = heroEntry.GetHeroStat();
        SetStatValue(0, heroData.Level);
        SetStatValue(1, heroStat.MaxHP);
        SetStatValue(2, heroStat.Atk);
        SetStatValue(3, heroStat.Def);
        SetStatValue(4, 0f);
        SetStatValue(5, heroEntry.SkillCooldown);


        if (heroFormationText != null)
        {
            switch (heroEntry.HeroLocation)
            {
                case HeroLocationEnum.Front:
                    heroFormationText.text = "전열";
                    break;
                case HeroLocationEnum.Back:
                    heroFormationText.text = "후열";
                    break;
                default:
                    heroFormationText.text = "알 수 없음";
                    break;
            }
        }

        if (heroNameText != null)
        {
            heroNameText.text = heroEntry.HeroName;
        }

        if (skillIcon != null)
        {
            skillIcon.sprite = heroEntry.SkillIcon;
            skillIcon.enabled = true;
        }
    }

    public void ClearHeroStatUIs()
    {
        if (heroStatUIs == null)
        {
            return;
        }

        currentHeroEntry = null;

        foreach (var statUI in heroStatUIs)
        {
            if (statUI != null)
            {
                statUI.Clear();
            }
        }

        if (skillIcon != null)
        {
            skillIcon.enabled = false;
        }

        if (heroFormationText != null)
        {
            heroFormationText.text = string.Empty;
        }

        if (heroNameText != null)
        {
            heroNameText.text = string.Empty;
        }
    }
    private void OnClickSkillIcon()
    {
        if (currentHeroEntry == null || skillToolTipUI == null) return;

        skillToolTipUI.SetToolTipData(
            currentHeroEntry.SkillIcon,
            currentHeroEntry.SkillName,
            currentHeroEntry.SkillCooldown,
            currentHeroEntry.SkillDescription
        );
    }

    private void SetStatValue(int index, double value)
    {
        if (heroStatUIs == null ||
            index < 0 ||
            index >= heroStatUIs.Length ||
            heroStatUIs[index] == null)
        {
            return;
        }

        heroStatUIs[index].SetValue(value);
    }

    private void SetStatValue(int index, int value)
    {
        if (heroStatUIs == null ||
            index < 0 ||
            index >= heroStatUIs.Length ||
            heroStatUIs[index] == null)
        {
            return;
        }

        heroStatUIs[index].SetValue(value);
    }
}

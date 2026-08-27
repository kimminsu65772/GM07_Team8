using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroArrangeStatUI : MonoBehaviour
{
    [SerializeField] private HeroStatUI[] heroStatUIs;
    [SerializeField] private TMP_Text heroFormationText;
    [SerializeField] private TMP_Text heroNameText;
    [SerializeField] private Image skillIcon;

    private HeroEntry currentHeroEntry;

    public void SetHeroStatUIs(HeroEntry heroEntry)
    {
        ClearHeroStatUIs();
        if (heroEntry == null)
        {
            return;
        }

        if (heroStatUIs == null)
        {
            Debug.LogError("스탯 UI 컴포넌트가 아직 연결되지 않았습니다.");
            return;
        }

        if (PlayerInfo.Instance == null ||
            !PlayerInfo.Instance.TryGetHeroData(heroEntry.HeroId, out HeroSaveData heroData))
        {
            Debug.LogWarning($"{heroEntry.HeroId}의 세이브 데이터를 찾을 수 없습니다.");
            return;
        }

        currentHeroEntry = heroEntry;

        HeroStat heroStat = heroEntry.GetHeroStat();
        SetStatValue(0, heroData.Level);
        SetStatValue(1, heroStat.MaxHP);
        SetStatValue(2, heroStat.Atk);
        SetStatValue(3, heroStat.Def);
        // TODO: 크리티컬은 장비 시스템 완성되면 적용
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

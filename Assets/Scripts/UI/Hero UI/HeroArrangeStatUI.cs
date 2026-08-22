using UnityEngine;
using UnityEngine.UI;

public class HeroArrangeStatUI : MonoBehaviour
{
    [SerializeField] private HeroStatUI[] heroStatUIs;
    [SerializeField] private Image skillIcon;

    public void SetHeroStatUIs(HeroEntry heroEntry)
    {
        ClearHeroStatUIs();
        if (heroEntry == null)
        {
            return;
        }

        if (heroStatUIs == null || heroStatUIs.Length < 6)
        {
            Debug.LogError("스탯 UI 컴포넌트가 아직 충분히 연결되지 않았습니다.");
            return;
        }

        if (PlayerInfo.Instance == null ||
            !PlayerInfo.Instance.TryGetHeroData(heroEntry.HeroId, out HeroSaveData heroData))
        {
            Debug.LogWarning($"{heroEntry.HeroId}의 세이브 데이터를 찾을 수 없습니다.");
            return;
        }

        HeroStat heroStat = heroEntry.GetHeroStat();
        SetStatValue(0, heroData.Level);
        SetStatValue(1, heroStat.MaxHP);
        SetStatValue(2, heroStat.Atk);
        SetStatValue(3, heroStat.Def);
        // TODO: 크리티컬은 장비 시스템 완성되면 적용
        SetStatValue(4, 0f);
        SetStatValue(5, heroEntry.SkillCooldown);

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
    }   

    private void SetStatValue(int index, float value)
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

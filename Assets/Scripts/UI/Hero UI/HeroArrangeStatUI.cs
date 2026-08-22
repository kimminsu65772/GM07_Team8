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
        heroStatUIs[0].SetValue(heroData.Level);
        heroStatUIs[1].SetValue(heroStat.MaxHP);
        heroStatUIs[2].SetValue(heroStat.Atk);
        heroStatUIs[3].SetValue(heroStat.Def);
        // TODO: 크리티컬은 장비 시스템 완성되면 적용
        heroStatUIs[4].SetValue(0f);
        heroStatUIs[5].SetValue(heroEntry.SkillCooldown);

        if (skillIcon != null)
        {
            skillIcon.sprite = heroEntry.SkillIcon;
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
    }   
}

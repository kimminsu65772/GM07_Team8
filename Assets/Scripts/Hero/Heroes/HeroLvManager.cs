using UnityEngine;

public class HeroLvManager : MonoBehaviour
{
    private static HeroLvManager instance;
    public static HeroLvManager Instance
    {
        get
        {
            if (instance == null) instance = new HeroLvManager();
            return instance;
        }
    }

    private Hero hero;
    private int heroMaxLv;
    private HeroStat stat;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LvUp()
    {
        // 비용 지불 조건/기능 추가
        if (hero.HeroLv >= heroMaxLv) return;

        hero.HeroLv++;
        LvApply(hero.HeroLv, hero);
    }

    public void LvSet(int lv)
    {
        if (lv >= heroMaxLv) lv = heroMaxLv;

        hero.HeroLv = lv;
        LvApply(hero.HeroLv, hero);
    }

    public void LvApply(int lv, Hero hero)
    {
        stat = hero.GetStat(lv);

        hero.HeroMaxHP = stat.MaxHP;
        hero.HeroAtk = stat.Atk;
        hero.HeroDef = stat.Def;
    }
}

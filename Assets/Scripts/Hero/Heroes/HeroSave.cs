using System.Collections.Generic;
using UnityEngine;

public struct HeroSaveInfo
{
    public int HeroID;
    public int HeroLv;

    public Equipment Weapon;
    public Equipment Body;
    public Equipment Acc;

    public int WeaponLv;
    public int BodyLv;
    public int AccLv;

    public HeroSaveInfo(int heroID, int heroLv, Equipment weapon, Equipment body, Equipment acc)
    {
        HeroID = heroID;
        HeroLv = heroLv;

        Weapon = weapon;
        Body = body;
        Acc = acc;

        WeaponLv = weapon.EquipLv;
        BodyLv = body.EquipLv;
        AccLv = acc.EquipLv;
    }
}

public class HeroSave : MonoBehaviour
{
    private HashSet<Hero> ownedHeroList;
    private Dictionary<int, HeroSaveInfo> heroSaveData;
    public HashSet<Hero> OwnedHeroList => ownedHeroList;
    public Dictionary<int, HeroSaveInfo> HeroSaveData => heroSaveData;

    public void AddHeroList(Hero hero)
    {
        if (ownedHeroList.Contains(hero)) return;

        ownedHeroList.Add(hero);
    }

    public void ClearHeroList()
    {
        ownedHeroList.Clear();
    }

    public HeroSaveInfo GetHeroSaveInfo(Hero hero)
    {
        Equipment[] equipInfo = hero.EquipInfo();

        return new HeroSaveInfo(
            hero.HeroID, hero.HeroLv,
            equipInfo[0], equipInfo[1], equipInfo[2]);
    }

    public void SaveHeroData()
    {
        foreach (Hero ownedHero in ownedHeroList)
        {
            heroSaveData[ownedHero.HeroID] = GetHeroSaveInfo(ownedHero);
        }
    }
}

using System.Collections.Generic;

public struct HeroStat
{
    public float MaxHP;
    public float Atk;
    public float Def;

    public HeroStat(float maxHP, float atk, float def)
    {
        MaxHP = maxHP;
        Atk = atk;
        Def = def;
    }
}

public interface IHeroStatTable
{
    HeroStat GetStat(int lv);
}

// 영웅1
public class Hero1StatTable : IHeroStatTable
{
    private readonly Dictionary<int, HeroStat> stats = new()
    {
        {1, new HeroStat(100f, 20f, 10f) },
        {2, new HeroStat(120, 25f, 12f) },
        {3, new HeroStat(140f, 30f, 14f) }
    };

    public HeroStat GetStat(int lv)
    {
        return stats[lv];
    }
}

// 영웅2
public class Hero2StatTable : IHeroStatTable
{
    private readonly Dictionary<int, HeroStat> stats = new()
    {
        {1, new HeroStat(80f, 10f, 7f) },
        {2, new HeroStat(90f, 17f, 8f) },
        {3, new HeroStat(100f, 24f, 9f) }
    };

    public HeroStat GetStat(int lv)
    {
        return stats[lv];
    }
}
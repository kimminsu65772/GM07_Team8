using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        {1, new HeroStat(100, 20, 10) },
        {2, new HeroStat(120, 25, 12) },
        {3, new HeroStat(140, 30, 14) }
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
        {1, new HeroStat(80, 10, 7) },
        {2, new HeroStat(90, 17, 8) },
        {3, new HeroStat(100, 24, 9) }
    };

    public HeroStat GetStat(int lv)
    {
        return stats[lv];
    }
}
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

public static class HeroStats
{
    public static readonly Dictionary<int, IHeroStatTable> HeroStatDic = new()
    {
        { -1, new Hero1StatTable() },
        { -2, new Hero2StatTable() }
    };

    public static IHeroStatTable GetStatTable(int id)
    {
        return HeroStatDic[id];
    }
}

// 영웅1
public class Hero1StatTable : IHeroStatTable
{
    private readonly Dictionary<int, HeroStat> stats = new()
    {
        {1, new HeroStat(150f, 50f, 20f) },
        {2, new HeroStat(200f, 70f, 30f) },
        {3, new HeroStat(250f, 90f, 40f) }
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
        {1, new HeroStat(100f, 50f, 10f) },
        {2, new HeroStat(120f, 70f, 15f) },
        {3, new HeroStat(140f, 90f, 20f) }
    };

    public HeroStat GetStat(int lv)
    {
        return stats[lv];
    }
}

// 영웅3
public class Hero3StatTable : IHeroStatTable
{
    private readonly Dictionary<int, HeroStat> stats = new()
    {
        {1, new HeroStat(90f, 50f, 10f) },
        {2, new HeroStat(110f, 70f, 15f) },
        {3, new HeroStat(130f, 90f, 20f) }
    };

    public HeroStat GetStat(int lv)
    {
        return stats[lv];
    }
}
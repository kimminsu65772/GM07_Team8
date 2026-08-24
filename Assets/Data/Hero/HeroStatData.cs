using System.Collections.Generic;

public struct HeroStat
{
    public double MaxHP;
    public double Atk;
    public double Def;

    public HeroStat(double maxHP, double atk, double def)
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
        { 11, new Hero11StatTable() },
        { 12, new Hero12StatTable() },
        { 13, new Hero13StatTable() },
        { 14, new Hero14StatTable() },
        { 15, new Hero15StatTable() },
        { 21, new Hero21StatTable() },
        { 22, new Hero22StatTable() },
        { 23, new Hero23StatTable() },
        { 24, new Hero24StatTable() },
        { 25, new Hero25StatTable() }
    };

    public static IHeroStatTable GetStatTable(int id)
    {
        return HeroStatDic[id];
    }
}

// 영웅11 Warrior
public class Hero11StatTable : IHeroStatTable
{
    private double lv1HP = 150;
    private double lv1Atk = 50;
    private double lv1Def = 5;

    private double lvHP = 50;
    private double lvAtk = 20;
    private double lvDef = 5;

    public HeroStat GetStat(int lv)
    {
        double HP = lv1HP + lvHP * (lv - 1);
        double Atk = lv1Atk + lvAtk * (lv - 1);
        double Def = lv1Def + lvDef * (lv - 1);

        return new HeroStat(HP, Atk, Def);
    }
}

// 영웅12 SpellBlade
public class Hero12StatTable : IHeroStatTable
{
    private double lv1HP = 130;
    private double lv1Atk = 50;
    private double lv1Def = 5;

    private double lvHP = 50;
    private double lvAtk = 20;
    private double lvDef = 5;

    public HeroStat GetStat(int lv)
    {
        double HP = lv1HP + lvHP * (lv - 1);
        double Atk = lv1Atk + lvAtk * (lv - 1);
        double Def = lv1Def + lvDef * (lv - 1);

        return new HeroStat(HP, Atk, Def);
    }
}

// 영웅13 Shieldman
public class Hero13StatTable : IHeroStatTable
{
    private double lv1HP = 150;
    private double lv1Atk = 50;
    private double lv1Def = 5;

    private double lvHP = 50;
    private double lvAtk = 20;
    private double lvDef = 5;

    public HeroStat GetStat(int lv)
    {
        double HP = lv1HP + lvHP * (lv - 1);
        double Atk = lv1Atk + lvAtk * (lv - 1);
        double Def = lv1Def + lvDef * (lv - 1);

        return new HeroStat(HP, Atk, Def);
    }
}

// 영웅14 Berserker
public class Hero14StatTable : IHeroStatTable
{
    private double lv1HP = 150;
    private double lv1Atk = 50;
    private double lv1Def = 5;

    private double lvHP = 50;
    private double lvAtk = 20;
    private double lvDef = 5;

    public HeroStat GetStat(int lv)
    {
        double HP = lv1HP + lvHP * (lv - 1);
        double Atk = lv1Atk + lvAtk * (lv - 1);
        double Def = lv1Def + lvDef * (lv - 1);

        return new HeroStat(HP, Atk, Def);
    }
}

// 영웅15 Rogue
public class Hero15StatTable : IHeroStatTable
{
    private double lv1HP = 150;
    private double lv1Atk = 50;
    private double lv1Def = 5;

    private double lvHP = 50;
    private double lvAtk = 20;
    private double lvDef = 5;

    public HeroStat GetStat(int lv)
    {
        double HP = lv1HP + lvHP * (lv - 1);
        double Atk = lv1Atk + lvAtk * (lv - 1);
        double Def = lv1Def + lvDef * (lv - 1);

        return new HeroStat(HP, Atk, Def);
    }
}

// 영웅21 Mage
public class Hero21StatTable : IHeroStatTable
{
    private double lv1HP = 100;
    private double lv1Atk = 50;
    private double lv1Def = 0;

    private double lvHP = 20;
    private double lvAtk = 20;
    private double lvDef = 2;

    public HeroStat GetStat(int lv)
    {
        double HP = lv1HP + lvHP * (lv - 1);
        double Atk = lv1Atk + lvAtk * (lv - 1);
        double Def = lv1Def + lvDef * (lv - 1);

        return new HeroStat(HP, Atk, Def);
    }
}

// 영웅22 Sorcery
public class Hero22StatTable : IHeroStatTable
{
    private double lv1HP = 90;
    private double lv1Atk = 50;
    private double lv1Def = 0;

    private double lvHP = 20;
    private double lvAtk = 20;
    private double lvDef = 2;

    public HeroStat GetStat(int lv)
    {
        double HP = lv1HP + lvHP * (lv - 1);
        double Atk = lv1Atk + lvAtk * (lv - 1);
        double Def = lv1Def + lvDef * (lv - 1);

        return new HeroStat(HP, Atk, Def);
    }
}

// 영웅23 RapidMage
public class Hero23StatTable : IHeroStatTable
{
    private double lv1HP = 100;
    private double lv1Atk = 20;
    private double lv1Def = 0;

    private double lvHP = 15;
    private double lvAtk = 8;
    private double lvDef = 2;

    public HeroStat GetStat(int lv)
    {
        double HP = lv1HP + lvHP * (lv - 1);
        double Atk = lv1Atk + lvAtk * (lv - 1);
        double Def = lv1Def + lvDef * (lv - 1);

        return new HeroStat(HP, Atk, Def);
    }
}

// 영웅24 Archer
public class Hero24StatTable : IHeroStatTable
{
    private double lv1HP = 100;
    private double lv1Atk = 50;
    private double lv1Def = 0;

    private double lvHP = 25;
    private double lvAtk = 25;
    private double lvDef = 3;

    public HeroStat GetStat(int lv)
    {
        double HP = lv1HP + lvHP * (lv - 1);
        double Atk = lv1Atk + lvAtk * (lv - 1);
        double Def = lv1Def + lvDef * (lv - 1);

        return new HeroStat(HP, Atk, Def);
    }
}

// 영웅25 RapidArcher
public class Hero25StatTable : IHeroStatTable
{
    private double lv1HP = 90;
    private double lv1Atk = 25;
    private double lv1Def = 0;

    private double lvHP = 15;
    private double lvAtk = 10;
    private double lvDef = 3;

    public HeroStat GetStat(int lv)
    {
        double HP = lv1HP + lvHP * (lv - 1);
        double Atk = lv1Atk + lvAtk * (lv - 1);
        double Def = lv1Def + lvDef * (lv - 1);

        return new HeroStat(HP, Atk, Def);
    }
}
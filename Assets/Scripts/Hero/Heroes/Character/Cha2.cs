using UnityEngine;

public class Cha2 : Hero
{
    protected override void Awake()
    {
        statTable = new Hero2StatTable();
        stat = statTable.GetStat(1);
        Init(1f, HeroLocationEnum.Back);
    }
}

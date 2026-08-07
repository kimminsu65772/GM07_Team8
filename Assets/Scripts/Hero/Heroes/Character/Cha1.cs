using UnityEngine;

public class Cha1 : Hero
{
    protected override void Awake()
    {
        statTable = new Hero1StatTable();
        stat = statTable.GetStat(1);
        Init(2f, HeroLocationEnum.Front);
    }
}

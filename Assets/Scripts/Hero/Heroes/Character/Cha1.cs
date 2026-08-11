using UnityEngine;

public class Cha1 : Hero
{
    protected override void Awake()
    {
        statTable = new Hero1StatTable();
        Init(-1, "Hero1", 2f, HeroLocationEnum.Front);
    }
}

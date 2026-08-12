using UnityEngine;

public class Cha2 : Hero
{
    protected override void Awake()
    {
        statTable = new Hero2StatTable();
        Init(-2, "Hero2", 1f, HeroLocationEnum.Back);
    }
}

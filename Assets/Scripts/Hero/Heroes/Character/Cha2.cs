using UnityEngine;

public class Cha2 : Hero
{
    protected override void Awake()
    {
        statTable = new Hero2StatTable();
        Init(-2, "Hero2", 1f, 5f, HeroLocationEnum.Back);
    }

    public override void Skill(GameObject enemy)
    {
        Debug.Log("원거리 스킬 사용");
    }
}
using UnityEngine;

public class Cha1 : Hero
{
    protected override void Awake()
    {
        statTable = new Hero1StatTable();
        Init(-1, "Hero1", 2f, 5f, HeroLocationEnum.Front);
    }

    public override void Skill()
    {
        Debug.Log("근거리 스킬 사용");
    }
}

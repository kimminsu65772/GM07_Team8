using UnityEngine;

public class Hero14_Berserker : Hero
{
    protected override void Awake()
    {
        statTable = new Hero14StatTable();
        SetAttackEffectPreset(0f, 0f, 1f, 1f);
        SetSkillEffectPreset(0f, 0f, 1f, 1f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(14, 1f, 6f, HeroLocationEnum.Front);
    }

    public override void Skill(GameObject enemy)
    {
        // 스킬 구현
        // 
    }
}

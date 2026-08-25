using UnityEngine;

public class Hero24_Archer : Hero
{
    protected override void Awake()
    {
        statTable = new Hero24StatTable();
        SetAttackEffectPreset(0f, 0f, 1f, 1f);
        SetSkillEffectPreset(0f, 0f, 1f, 1f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(24, 1f, 6f, HeroLocationEnum.Back);
    }

    public override void Skill(GameObject enemy)
    {
        // 스킬 구현
        // 강타
    }
}

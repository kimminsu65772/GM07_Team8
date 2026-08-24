using UnityEngine;

public class Hero12_SpellBlade : Hero
{
    protected override void Awake()
    {
        statTable = new Hero12StatTable();
        SetAttackEffectPreset(0f, 0f, 0f, 0f);
        SetSkillEffectPreset(0f, 0f, 0f, 0f);
        SetTargetEffectPreset(0f, 0f, 0f, 0f);
        Init(12, 1f, 6f, HeroLocationEnum.Front);
    }

    public override void Skill(GameObject enemy)
    {
        // 스킬 구현
    }
}

using UnityEngine;

public class Hero23_RapidMage : Hero
{
    protected override void Awake()
    {
        statTable = new Hero23StatTable();
        SetAttackEffectPreset(0f, 0f, 0f, 0f);
        SetSkillEffectPreset(0f, 0f, 0f, 0f);
        SetTargetEffectPreset(0f, 0f, 0f, 0f);
        Init(23, 1f, 6f, HeroLocationEnum.Back);
    }

    public override void Skill(GameObject enemy)
    {
        // 스킬 구현
    }
}

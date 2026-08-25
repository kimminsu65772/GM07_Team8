using UnityEngine;

public class Hero23_RapidMage : Hero
{
    protected override void Awake()
    {
        statTable = new Hero23StatTable();
        SetAttackEffectPreset(0f, 0f, 1f, 1f);
        SetSkillEffectPreset(0f, 0f, 1f, 1f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(23, 0.5f, 6f, HeroLocationEnum.Back);
    }

    public override void Skill(GameObject enemy)
    {
        // 스킬 구현
        // 셀프 공속 버프
    }
}

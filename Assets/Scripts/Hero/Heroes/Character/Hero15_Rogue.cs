using UnityEngine;

public class Hero15_Rogue : Hero
{
    protected override void Awake()
    {
        statTable = new Hero15StatTable();
        SetAttackEffectPreset(0f, 0f, 1f, 1f);
        SetSkillEffectPreset(0f, 0f, 1f, 1f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(15, 1f, 6f, HeroLocationEnum.Front);
    }

    public override void Skill(GameObject enemy)
    {
        // 스킬 구현 버프
        // 셀프 공속 버프
    }
}

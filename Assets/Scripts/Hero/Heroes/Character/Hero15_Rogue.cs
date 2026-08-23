using UnityEngine;

public class Hero15_Rogue : Hero
{
    protected override void Awake()
    {
        statTable = new Hero2StatTable();
        SetAttackEffectPreset(0f, 0f, 0f, 0f);
        SetSkillEffectPreset(0f, 0f, 0f, 0f);
        SetTargetEffectPreset(0f, 0f, 0f, 0f);
        Init(15, 1f, 6f, HeroLocationEnum.Front);
    }

    public override void Skill(GameObject enemy)
    {
        // 스킬 구현 버프
    }
}

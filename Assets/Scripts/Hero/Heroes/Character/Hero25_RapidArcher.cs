using UnityEngine;

public class Hero25_RapidArcher : Hero
{
    protected override void Awake()
    {
        statTable = new Hero25StatTable();
        SetAttackEffectPreset(0f, 0f, 1f, 1f);
        SetSkillEffectPreset(0f, 0f, 1f, 1f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(25, 0.5f, 8f, HeroLocationEnum.Back);
    }

    public override void Skill(GameObject enemy)
    {
        if (enemy == null || IsDead) return;

        // 스킬 구현
        // 셀프 공속 버프
    }
}

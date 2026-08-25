using UnityEngine;

public class Hero13_Shieldman : Hero
{
    protected override void Awake()
    {
        statTable = new Hero13StatTable();
        SetAttackEffectPreset(-0.3f, 0.4f, -1.5f, 1.5f);
        SetSkillEffectPreset(0f, 0f, 1f, 1f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(13, 1f, 6f, HeroLocationEnum.Front);
    }

    public override void Skill(GameObject enemy)
    {
        // 스킬 구현
        // 타겟팅 중인 적 기절 (보스 적용 다르게?)
    }
}

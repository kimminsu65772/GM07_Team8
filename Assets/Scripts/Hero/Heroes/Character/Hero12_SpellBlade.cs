using UnityEngine;

public class Hero12_SpellBlade : Hero
{
    protected override void Awake()
    {
        statTable = new Hero12StatTable();
        SetAttackEffectPreset(-0.65f, 0.5f, -1.4f, 1.7f);
        SetSkillEffectPreset(0f, 0.5f, 3f, 3f);
        SetTargetEffectPreset(0f, 1f, 6f, 6f);
        Init(12, 1f, 6f, HeroLocationEnum.Front);
    }

    public override void Skill(GameObject enemy)
    {
        if (enemy == null || IsDead) return;

        attack.AreaAttack(0, enemy.transform, 3f, 1.2f);
    }
}

using UnityEngine;

public class Hero22_Sorcery : Hero
{
    protected override void Awake()
    {
        statTable = new Hero22StatTable();
        SetAttackEffectPreset(0f, 0.5f, 1.5f, 1.5f);
        SetSkillEffectPreset(0f, 0.5f, 1.5f, 1.7f);
        SetTargetEffectPreset(0f, 1.4f, 7f, 7f);
        Init(22, 1f, 5f, HeroLocationEnum.Back);
    }

    public override void Skill(GameObject enemy)
    {
        if (enemy == null || IsDead) return;

        Attack.AreaAttack(1, enemy.transform, 5f, 1.2f);
    }
}

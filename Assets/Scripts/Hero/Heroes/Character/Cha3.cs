using UnityEngine;

public class Cha3 : Hero
{
    protected override void Awake()
    {
        statTable = new Hero3StatTable();
        SetAttackEffectPreset(0f, 0.5f, 1.5f, 1.5f);
        SetSkillEffectPreset(0f, 0.5f, 1.5f, 1.7f);
        SetTargetEffectPreset(0f, 1.4f, 7f, 7f);
        Init(3, 1f, 5f, HeroLocationEnum.Back);

        EditSkillText(
            "물보라",
            "큰 물보라를 일으켜 대상 주변의 적에게 피해를 입힙니다."
            );
    }

    public override void Skill(GameObject enemy)
    {
        attack.AreaAttack(1, enemy.transform, 5f, 1.2f);
    }
}

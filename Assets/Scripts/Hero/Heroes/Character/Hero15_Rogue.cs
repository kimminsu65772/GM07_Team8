using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Hero15_Rogue : Hero
{
    private Coroutine attackTimeBuffCo;

    protected override void Awake()
    {
        statTable = new Hero15StatTable();
        SetAttackEffectPreset(0f, 0f, 1f, 1f);
        SetSkillEffectPreset(0f, 0f, 1f, 1f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(15, 0.5f, 10f, HeroLocationEnum.Front);
    }

    public override void Skill(GameObject enemy)
    {
        if (enemy == null || IsDead) return;

        attackTimeBuffCo = StartCoroutine(AttackTimeBuff());
        attack.VFX.PlaySkillEffect(SkillPosPreset, SkillScalePreset);
    }

    private IEnumerator AttackTimeBuff()
    {
        float buffedAttackTime = 0.25f;
        float buffTime = 5f;
        float attackTimeTmp = HeroAttackTime;

        SetAttackTime(buffedAttackTime);

        yield return new WaitForSeconds(buffTime);

        SetAttackTime(attackTimeTmp);

    }
}

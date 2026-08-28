using System.Collections;
using UnityEngine;

public class Hero25_RapidArcher : Hero
{
    private Coroutine attackTimeBuffCo;
    private float attackBuffAmount = 2.5f;
    private float originalAttackTime = 1f;
    [SerializeField] private Animator animator;

    protected override void Awake()
    {
        statTable = new Hero25StatTable();
        SetAttackEffectPreset(0f, 0f, 1f, 1f);
        SetSkillEffectPreset(0f, 0f, 1f, 1f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(25, originalAttackTime, 10f, HeroLocationEnum.Back);
    }

    public override void Skill(GameObject enemy)
    {
        if (enemy == null || IsDead) return;

        if (attackTimeBuffCo != null) StopCoroutine(attackTimeBuffCo);

        attackTimeBuffCo = StartCoroutine(AttackTimeBuff());
        Attack.VFX.PlaySkillEffect(SkillPosPreset, SkillScalePreset);
    }

    private IEnumerator AttackTimeBuff()
    {
        float buffedAttackTime = originalAttackTime / attackBuffAmount;

        SetAttackTime(buffedAttackTime);
        animator.SetFloat("bonusSpeed", attackBuffAmount);

        yield return new WaitForSeconds(5f);

        SetAttackTime(originalAttackTime);
        animator.SetFloat("bonusSpeed", 1f);

        attackTimeBuffCo = null;
    }

    private void OnDisable()
    {
        if (attackTimeBuffCo != null)
        {
            StopCoroutine(attackTimeBuffCo);
            attackTimeBuffCo = null;
        }

        HeroAttackTime = originalAttackTime;
        animator.SetFloat("bonusSpeed", 1f);
    }
}

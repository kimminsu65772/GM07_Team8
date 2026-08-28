using System.Collections;
using UnityEngine;

public class Hero23_RapidMage : Hero
{
    private Coroutine attackTimeBuffCo;
    private float attackBuffAmount = 1.5f;
    private float originalAttackTime = 0.7f;
    [SerializeField] private Animator animator;

    public bool IsAdditionalAttackActive { get; private set; }

    protected override void Awake()
    {
        statTable = new Hero23StatTable();
        SetAttackEffectPreset(0f, 0f, 1f, 1f);
        SetSkillEffectPreset(0f, 0f, 1f, 1f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(23, originalAttackTime, 8f, HeroLocationEnum.Back);
        animator.SetFloat("bonusSpeed", originalAttackTime);
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
        IsAdditionalAttackActive = true;

        yield return new WaitForSeconds(5f);

        SetAttackTime(originalAttackTime);
        animator.SetFloat("bonusSpeed", originalAttackTime);

        IsAdditionalAttackActive = false;

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
        IsAdditionalAttackActive = false;
        animator.SetFloat("bonusSpeed", originalAttackTime);
    }
}

using System.Collections;
using UnityEngine;

public class Hero14_Berserker : Hero
{
    private Coroutine berserkerBuffCo;
    private float attackBuffAmount = 1.3f;
    private float originalAttackTime = 0.9f;
    private float originalCriChance;
    [SerializeField] private Animator animator;

    protected override void Awake()
    {
        statTable = new Hero14StatTable();
        SetAttackEffectPreset(-0.6f, 0.5f, -1.7f, 1.4f);
        SetSkillEffectPreset(0f, 0.4f, 3f, 3f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(14, originalAttackTime, 10f, HeroLocationEnum.Front);
    }

    public override void Skill(GameObject enemy)
    {
        if (enemy == null || IsDead) return;

        if (berserkerBuffCo != null) StopCoroutine(berserkerBuffCo);

        berserkerBuffCo = StartCoroutine(BerserkerBuff());
        Attack.VFX.PlaySkillEffect(SkillPosPreset, SkillScalePreset);
    }

    private IEnumerator BerserkerBuff()
    {
        float buffedAttackTime = originalAttackTime / attackBuffAmount;
        originalCriChance = HeroCriChance;

        SetAttackTime(buffedAttackTime);
        SetCriChance(100f);
        Attack.VFX.ChangeFrames();
        SetAttackEffectPreset(-0.6f, 0.3f, -2.3f, 2f);
        animator.SetFloat("bonusSpeed", attackBuffAmount);

        yield return new WaitForSeconds(5f);

        SetAttackTime(originalAttackTime);
        SetCriChance(originalCriChance);
        Attack.VFX.ChangeFrames();
        SetAttackEffectPreset(-0.6f, 0.5f, -1.7f, 1.4f);
        animator.SetFloat("bonusSpeed", 1f);

        berserkerBuffCo = null;
    }

    private void OnDisable()
    {
        if (berserkerBuffCo != null)
        {
            StopCoroutine(berserkerBuffCo);
            berserkerBuffCo = null;
        }

        SetAttackTime(originalAttackTime);
        SetCriChance(originalCriChance);
        Attack.VFX.ChangeFrames();
        SetAttackEffectPreset(-0.6f, 0.5f, -1.7f, 1.4f);
        animator.SetFloat("bonusSpeed", 1f);
    }
}

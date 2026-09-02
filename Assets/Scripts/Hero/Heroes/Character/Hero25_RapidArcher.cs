using System.Collections;
using UnityEngine;

public class Hero25_RapidArcher : Hero
{
    private Coroutine attackTimeBuffCo;
    private float attackBuffAmount = 2.5f;
    private float originalAttackTime = 1f;
    [SerializeField] private Animator animator;
    private StageManager stageManager;

    protected override void Awake()
    {
        statTable = new Hero25StatTable();
        SetAttackEffectPreset(0f, 0f, 1f, 1f);
        SetSkillEffectPreset(0f, 0f, 1f, 1f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(25, originalAttackTime, 10f, HeroLocationEnum.Back);
        stageManager = FindFirstObjectByType<StageManager>();
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
        animator.SetFloat("bonusSpeed", attackBuffAmount * 1.5f);

        yield return new WaitForSeconds(5f);

        ClearBuff();
    }

    private void ClearBuff()
    {
        SetAttackTime(originalAttackTime);
        animator.SetFloat("bonusSpeed", 1f);
        attackTimeBuffCo = null;
    }

    private void HandleWaveCompleted(int wave)
    {
        ClearBuff();
    }

    private void OnEnable()
    {
        stageManager.OnWaveCompleted += HandleWaveCompleted;
    }

    private void OnDisable()
    {
        stageManager.OnWaveCompleted -= HandleWaveCompleted;

        if (attackTimeBuffCo != null)
        {
            StopCoroutine(attackTimeBuffCo);
            ClearBuff();
        }
    }
}

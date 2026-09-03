using System.Collections;
using UnityEngine;

public class Hero23_RapidMage : Hero
{
    private Coroutine attackTimeBuffCo;
    private float attackBuffAmount = 1.5f;
    private float originalAttackTime = 0.7f;
    [SerializeField] private Animator animator;
    private StageManager stageManager;

    public bool IsAdditionalAttackActive { get; private set; }

    protected override void Awake()
    {
        statTable = new Hero23StatTable();
        SetAttackEffectPreset(0f, 0f, 1f, 1f);
        SetSkillEffectPreset(0f, 0f, 1f, 1f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(23, originalAttackTime, 8f, HeroLocationEnum.Back);
        animator.SetFloat("bonusSpeed", originalAttackTime);
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
        animator.SetFloat("bonusSpeed", attackBuffAmount * 1.8f);
        IsAdditionalAttackActive = true;

        yield return new WaitForSeconds(5f);

        ClearBuff();
    }

    private void ClearBuff()
    {
        if (attackTimeBuffCo == null) return;

        SetAttackTime(originalAttackTime);
        animator.SetFloat("bonusSpeed", originalAttackTime);
        IsAdditionalAttackActive = false;
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

using System.Collections;
using UnityEngine;

public class Hero15_Rogue : Hero
{
    private Coroutine attackTimeBuffCo;
    private float attackBuffAmount = 2f;
    private float originalAttackTime = 0.5f;
    [SerializeField] private Animator animator;
    private StageManager stageManager;

    protected override void Awake()
    {
        statTable = new Hero15StatTable();
        SetAttackEffectPreset(-0.5f, 0.3f, -1.5f, 1.5f);
        SetSkillEffectPreset(0f, 0.2f, 3f, 3f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(15, originalAttackTime, 10f, HeroLocationEnum.Front);
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
        Attack.VFX.ChangeFrames();
        SetAttackEffectPreset(-0.5f, 0.3f, -2.5f, 2.5f);

        yield return new WaitForSeconds(5f);

        ClearBuff();
    }

    private void ClearBuff()
    {
        if (attackTimeBuffCo == null) return;

        SetAttackTime(originalAttackTime);
        animator.SetFloat("bonusSpeed", 1f);
        Attack.VFX.ChangeFrames();
        SetAttackEffectPreset(-0.5f, 0.3f, -1.5f, 1.5f);
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

using System.Collections;
using UnityEngine;

public class Hero14_Berserker : Hero
{
    private Coroutine berserkerBuffCo;
    private float attackBuffAmount = 1.3f;
    private float originalAttackTime = 0.9f;
    private float originalCriChance;
    [SerializeField] private Animator animator;
    [SerializeField] private StageManager stageManager;

    protected override void Awake()
    {
        statTable = new Hero14StatTable();
        SetAttackEffectPreset(-0.6f, 0.5f, -1.7f, 1.4f);
        SetSkillEffectPreset(0f, 0.7f, 3f, 3f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);
        Init(14, originalAttackTime, 12f, HeroLocationEnum.Front);
        stageManager = FindFirstObjectByType<StageManager>();
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
        animator.SetFloat("bonusSpeed", attackBuffAmount * 1.5f);

        yield return new WaitForSeconds(4f);

        ClearBuff();
    }

    private void ClearBuff()
    {
        if (berserkerBuffCo == null) return;

        SetAttackTime(originalAttackTime);
        SetCriChance(originalCriChance);
        Attack.VFX.ChangeFrames();
        SetAttackEffectPreset(-0.6f, 0.5f, -1.7f, 1.4f);
        animator.SetFloat("bonusSpeed", 1f);
        berserkerBuffCo = null;
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

        if (berserkerBuffCo != null)
        {
            StopCoroutine(berserkerBuffCo);
            ClearBuff();
        }
    }
}

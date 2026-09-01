using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(BossChargeSkill))]
[RequireComponent(typeof(BossDotAreaSkill))]
[RequireComponent(typeof(BossRangedBarrageSkill))]
public class FinalBossSkillController : MonoBehaviour
{
    [Header("Skill Cycle")]
    [SerializeField] private float firstSkillDelay = 3f;
    [SerializeField] private float skillInterval = 3f;

    [Header("Phase")]
    [SerializeField, Range(0f, 1f)] private float dotUnlockHealthRatio = 0.7f;
    [SerializeField, Range(0f, 1f)] private float barrageUnlockHealthRatio = 0.4f;

    private EnemyStats enemyStats;
    private BossChargeSkill chargeSkill;
    private BossDotAreaSkill dotAreaSkill;
    private BossRangedBarrageSkill barrageSkill;

    private bool isUsingSkill;
    private int nextSkillIndex;
    private float nextSkillTime;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
        chargeSkill = GetComponent<BossChargeSkill>();
        dotAreaSkill = GetComponent<BossDotAreaSkill>();
        barrageSkill = GetComponent<BossRangedBarrageSkill>();

        // 개별 보스 스킬의 자동 발동을 끄고 이 컨트롤러가 순서를 관리한다.
        chargeSkill.SetAutoUse(false);
        dotAreaSkill.SetAutoUse(false);
        barrageSkill.SetAutoUse(false);
    }

    private void OnEnable()
    {
        isUsingSkill = false;
        nextSkillIndex = 0;
        nextSkillTime = Time.time + firstSkillDelay;
    }

    private void Update()
    {
        if (enemyStats == null ||
            enemyStats.IsDead ||
            isUsingSkill ||
            Time.time < nextSkillTime)
        {
            return;
        }

        StartCoroutine(UseNextSkill());
    }

    private IEnumerator UseNextSkill()
    {
        isUsingSkill = true;

        if (!TryUseNextSkill())
        {
            nextSkillTime = Time.time + 1f;
            isUsingSkill = false;
            yield break;
        }

        while (chargeSkill.IsUsingSkill ||
               dotAreaSkill.IsCasting ||
               barrageSkill.IsCasting)
        {
            yield return null;
        }

        nextSkillTime = Time.time + skillInterval;
        isUsingSkill = false;
    }

    private bool TryUseNextSkill()
    {
        float healthRatio =
            enemyStats.CurrentHealth /
            enemyStats.MaxHealth;

        if (healthRatio > dotUnlockHealthRatio)
        {
            return chargeSkill.UseSkill();
        }

        if (healthRatio > barrageUnlockHealthRatio)
        {
            bool useCharge =
                nextSkillIndex % 2 == 0;

            nextSkillIndex++;

            return useCharge
                ? chargeSkill.UseSkill()
                : dotAreaSkill.UseSkill();
        }

        int skillIndex =
            nextSkillIndex % 3;

        nextSkillIndex++;

        if (skillIndex == 0)
        {
            return chargeSkill.UseSkill();
        }

        if (skillIndex == 1)
        {
            return dotAreaSkill.UseSkill();
        }

        return barrageSkill.UseSkill();
    }
    private void OnDisable()
    {
        isUsingSkill = false;
    }
}
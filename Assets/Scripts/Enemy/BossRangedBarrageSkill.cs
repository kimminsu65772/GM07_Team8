using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyRangedAttack))]
public class BossRangedBarrageSkill : MonoBehaviour
{
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private float firstSkillDelay = 3f;
    [SerializeField] private float skillCooldown = 8f;
    [SerializeField] private float shotInterval = 0.2f;
    [SerializeField] private int shotCount = 3;
    [SerializeField] private float animationLeadTime = 0.3f;
    [SerializeField] private Transform skillFirePoint;

    private EnemyStats enemyStats;
    private EnemyRangedAttack enemyRangedAttack;
    private EnemyAttack enemyAttack;
    private bool isCasting;
    private float nextSkillTime;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
        enemyRangedAttack = GetComponent<EnemyRangedAttack>();
        enemyAttack = GetComponent<EnemyAttack>();

        if (enemyAnimator == null) enemyAnimator = GetComponentInChildren<Animator>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(firstSkillDelay);
        nextSkillTime = Time.time;
    }

    private void Update()
    {
        if (isCasting || enemyStats.IsDead || Time.time < nextSkillTime) return;

        StartCoroutine(FireBarrage());
    }

    private IEnumerator FireBarrage()
    {
        isCasting = true;
        enemyAttack.enabled = false;
        enemyRangedAttack.enabled = false;

        enemyAnimator.SetTrigger("6_Other");

        yield return new WaitForSeconds(animationLeadTime);

        for (int i = 0; i < shotCount; i++)
        {
            if (enemyStats.IsDead) break;

            enemyRangedAttack.FireProjectile(skillFirePoint);

            if (i < shotCount - 1) yield return new WaitForSeconds(shotInterval);
        }

        enemyAttack.enabled = true;
        enemyRangedAttack.enabled = true;
        nextSkillTime = Time.time + skillCooldown;
        isCasting = false;
    }
}
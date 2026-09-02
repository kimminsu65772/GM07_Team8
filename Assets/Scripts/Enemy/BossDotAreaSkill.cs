using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class BossDotAreaSkill : MonoBehaviour
{
    [Header("Skill")]
    [SerializeField, Min(0.1f)] private float skillCooldown = 8f;
    [SerializeField, Min(0f)] private float firstSkillDelay = 3f;
    [SerializeField, Min(0.1f)] private float skillAnimationDuration = 1.5f;

    [Header("Dot Area")]
    [SerializeField] private DotDamageArea dotAreaPrefab;
    [SerializeField] private Transform areaSpawnPoint;
    [SerializeField] private Vector3 areaPositionOffset;

    private EnemyMovement enemyMovement;
    private EnemyStats enemyStats;
    private EnemyAttack enemyAttack;
    private EnemyAnimationController animationController;

    private Coroutine skillCoroutine;
    private bool isCasting;
    private bool isAutoUse = true;

    public bool IsCasting => isCasting;
    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
        enemyAttack = GetComponent<EnemyAttack>();
        animationController = GetComponent<EnemyAnimationController>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    private void OnEnable()
    {
        if (isAutoUse)
        {
            skillCoroutine = StartCoroutine(SkillRoutine());
        }
    }
    // 최종 보스 컨트롤러에서 자동 발동을 끈다.
    public void SetAutoUse(bool value)
    {
        isAutoUse = value;

        if (!isAutoUse && skillCoroutine != null)
        {
            StopCoroutine(skillCoroutine);
            skillCoroutine = null;
        }
    }

    // 최종 보스 컨트롤러가 장판 스킬을 직접 실행한다.
    public bool UseSkill()
    {
        if (isCasting || enemyStats == null || enemyStats.IsDead || dotAreaPrefab == null)
        {
            return false;
        }

        StartCoroutine(UseSkillRoutine());
        return true;
    }
    private IEnumerator SkillRoutine()
    {
        yield return new WaitForSeconds(firstSkillDelay);

        while (enemyStats != null && !enemyStats.IsDead)
        {
            yield return StartCoroutine(UseSkillRoutine());

            yield return new WaitForSeconds(skillCooldown);
        }
    }
    private IEnumerator UseSkillRoutine()
    {
        isCasting = true;

        // 광역기 시전 중에는 기본 공격이 함께 실행되지 않도록 막는다.
        if (enemyAttack != null)
        {
            enemyAttack.enabled = false;
        }

        animationController?.PlayDotAreaSkill();

        yield return new WaitForSeconds(skillAnimationDuration);

        if (enemyAttack != null)
        {
            enemyAttack.enabled = true;
        }

        isCasting = false;
    }
    // 광역기 애니메이션 이벤트에서 호출한다.
    public void CastDotArea()
    {
        if (!isCasting || enemyStats == null || enemyStats.IsDead || dotAreaPrefab == null)
        {
            return;
        }

        if (enemyMovement == null || enemyMovement.AirshipTarget == null)
        {
            return;
        }

        Transform airshipTarget = enemyMovement.AirshipTarget;
        Vector3 spawnPosition = airshipTarget.position + areaPositionOffset;

        DotDamageArea area = Instantiate(dotAreaPrefab, spawnPosition, Quaternion.identity);
        area.transform.SetParent(airshipTarget, true);

        area.Initialize(enemyStats.AttackPower);
    }

    private void OnDisable()
    {
        if (skillCoroutine != null)
        {
            StopCoroutine(skillCoroutine);
            skillCoroutine = null;
        }

        isCasting = false;

        // 시전 도중 풀로 반환돼도 기본 공격이 비활성화된 채 남지 않도록 복구한다.
        if (enemyAttack != null)
        {
            enemyAttack.enabled = true;
        }
    }
}
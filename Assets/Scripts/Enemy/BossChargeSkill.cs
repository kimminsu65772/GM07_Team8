using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyStats))]
public class BossChargeSkill : MonoBehaviour
{
    [Header("Charge")]
    [SerializeField] private float chargeDistance = 5f;
    [SerializeField] private float chargeDuration = 0.08f;
    [SerializeField] private float chargeCooldown = 6f;
    [SerializeField] private float teleportDelay = 0.04f;

    [Header("Damage")]
    [SerializeField] private float chargeDamageMultiplier = 2f;
    [SerializeField] private float hitRadius = 1.2f;
    [SerializeField] private LayerMask targetLayer;

    private Rigidbody2D enemyRigidbody2D;
    private EnemyStats enemyStats;
    private EnemyAttack enemyAttack;
    private EnemyMovement enemyMovement;
    private EnemyAnimationController enemyAnimationController;

    private bool isUsingSkill;
    private bool isCharging;
    private bool hasHitTarget;
    private bool isAutoUse = true;

    public bool IsUsingSkill => isUsingSkill;

    private float skillTimer;
    private Vector2 originPosition;

    private void Awake()
    {
        enemyRigidbody2D = GetComponent<Rigidbody2D>();

        enemyStats =  GetComponent<EnemyStats>();

        enemyAttack = GetComponent<EnemyAttack>();

        enemyMovement =GetComponent<EnemyMovement>();

        enemyAnimationController =GetComponent<EnemyAnimationController>();
    }

    private void Start()
    {
        skillTimer = chargeCooldown;
    }
    // 최종 보스 컨트롤러에서 자동 발동을 끈다.
    public void SetAutoUse(bool value)
    {
        isAutoUse = value;
    }

    // 최종 보스 컨트롤러가 돌진을 직접 실행한다.
    public bool UseSkill()
    {
        if (enemyStats == null || enemyStats.IsDead ||   isUsingSkill)
        {
            return false;
        }

        PrepareCharge();
        return true;
    }
    private void Update()
    {
        if (!isAutoUse || enemyStats == null ||enemyStats.IsDead ||isUsingSkill)
        {
            return;
        }

        skillTimer -=  Time.deltaTime;

        if (skillTimer <= 0f)
        {
            PrepareCharge();
        }
    }

    private void PrepareCharge()
    {
        Debug.Log("보스 몸통박치기 준비");

        isUsingSkill = true;

        // 스킬마다 데미지 판정 초기화
        hasHitTarget = false;

        skillTimer = chargeCooldown;

        // 스킬 시작 위치 저장
        originPosition = enemyRigidbody2D.position;

        // 스킬 중 일반 공격 중지
        if (enemyAttack != null)
        {
            enemyAttack.enabled = false;
        }

        // 스킬 중 일반 이동 중지
        if (enemyMovement != null)
        {
            enemyMovement.enabled = false;
        }

        enemyRigidbody2D.linearVelocity =  Vector2.zero;

        // 몸통박치기 모션 시작
        if (enemyAnimationController != null)
        {
            enemyAnimationController.PlayCharge();
        }
    }

    // Animation Event에서 호출
    public void StartCharge()
    {
        Debug.Log("StartCharge Animation Event 호출");
        if (!isUsingSkill ||isCharging)
        {
            return;
        }

        StartCoroutine( ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        Debug.Log("몸통박치기 실제 돌진 시작");
        isCharging = true;

        Vector2 chargeTarget = originPosition +  Vector2.left * chargeDistance;

        // 돌진 경로 데미지 판정
        CheckChargeHit();

        float elapsedTime = 0f;

        // 빠르게 돌진
        while (elapsedTime < chargeDuration)
        {
            elapsedTime +=  Time.fixedDeltaTime;

            float progress =  Mathf.Clamp01(  elapsedTime /  chargeDuration  );

            enemyRigidbody2D.position =  Vector2.Lerp( originPosition,  chargeTarget,  progress  );

            yield return new WaitForFixedUpdate();
        }

        enemyRigidbody2D.position =    chargeTarget;

        // 타격 순간만 짧게 유지
        yield return new WaitForSeconds(  teleportDelay );

        // 원래 위치로 순간이동
        enemyRigidbody2D.position = originPosition;

        enemyRigidbody2D.linearVelocity = Vector2.zero;

        isCharging = false;
        isUsingSkill = false;

        // 일반 이동 재개
        if (enemyMovement != null)
        {
            enemyMovement.enabled = true;
        }

        // 일반 공격 재개
        if (enemyAttack != null)
        {
            enemyAttack.enabled = true;
        }
    }

    private void CheckChargeHit()
    {
        if (hasHitTarget)
        {
            return;
        }

        RaycastHit2D[] hits =  Physics2D.CircleCastAll( originPosition,   hitRadius,   Vector2.left,  chargeDistance,  targetLayer  );

        if (hits.Length == 0)
        {
            Debug.Log( "몸통박치기 대상 없음"  );

            return;
        }

        float chargeDamage = enemyStats.AttackPower *  chargeDamageMultiplier;

        // 콜라이더가 여러 개인 대상을 중복 공격하지 않도록 저장
        HashSet<IDamageable> damagedTargets =
            new HashSet<IDamageable>();

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            IDamageable target =  hit.collider.GetComponentInParent<IDamageable>();

            if (target == null || damagedTargets.Contains(target))
            {
                continue;
            }

            damagedTargets.Add(target);

            target.TakeDamage(  new DamageInfo(   chargeDamage )   );

            Debug.Log( $"몸통박치기 적중! 대상: " + $"{hit.collider.name}, " +   $"데미지: {chargeDamage}" );
        }

        if (damagedTargets.Count > 0)
        {
            hasHitTarget = true;
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();

        isUsingSkill = false;
        isCharging = false;
        hasHitTarget = false;

        if (enemyMovement != null)
        {
            enemyMovement.enabled = true;
        }

        if (enemyAttack != null)
        {
            enemyAttack.enabled = true;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere( transform.position,  hitRadius  );
    }
}
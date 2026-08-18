using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyStats))]
public class EnemyAttack : MonoBehaviour
{
    private IDamageable targetDamageable;

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Attack Event")]
    [SerializeField]
    private UnityEvent attackPerformed = new UnityEvent();

    [SerializeField] private SpriteRenderer spriteRenderer;

    private EnemyStats enemyStats;
    private float attackTimer;

    // 기본 1배
    private float attackSpeedMultiplier = 1f;

    private void Awake()
    {
        enemyStats =
            GetComponent<EnemyStats>();

        ResetAttackTimer();
    }

    private void Update()
    {
        // 타깃이 없거나 적이 죽었으면 공격 중단
        if (target == null ||
            targetDamageable == null ||
            enemyStats == null ||
            enemyStats.IsDead)
        {
            ResetAttackTimer();
            return;
        }

        float horizontalDistanceToTarget =
            Mathf.Abs(
                transform.position.x -
                target.position.x
            );

        // 공격 범위 밖이면 공격하지 않음
        if (horizontalDistanceToTarget >
            enemyStats.AttackRange)
        {
            ResetAttackTimer();
            return;
        }

        // 현재 공격 대상을 바라봄
        FaceTarget();

        attackTimer -=
            Time.deltaTime;

        if (attackTimer > 0f)
        {
            return;
        }

        Attack();
        ResetAttackTimer();
    }

    // 공격속도 배율 변경
    public void SetAttackSpeedMultiplier(
        float multiplier)
    {
        attackSpeedMultiplier =
            Mathf.Max(
                multiplier,
                0.1f
            );

        // 변경된 공속 바로 적용
        ResetAttackTimer();
    }

    public void SetTarget(
        Transform newTarget)
    {
        target =
            newTarget;

        targetDamageable =
            target != null
                ? target.GetComponentInParent<IDamageable>()
                : null;

        if (target != null &&
            targetDamageable == null)
        {
            Debug.LogWarning(
                $"{name}: {target.name}에서 IDamageable을 찾지 못했습니다."
            );
        }

        ResetAttackTimer();
    }

    private void Attack()
    {
        if (target == null ||
            targetDamageable == null)
        {
            return;
        }

        FaceTarget();

        attackPerformed?.Invoke();
    }

    // Animation Event에서 실제 데미지 적용
    public void ApplyAttackDamage()
    {
        if (target == null ||
            targetDamageable == null ||
            enemyStats == null ||
            enemyStats.IsDead)
        {
            return;
        }

        targetDamageable.TakeDamage(
            enemyStats.AttackPower
        );
    }

    private void FaceTarget()
    {
        if (target == null ||
            spriteRenderer == null)
        {
            return;
        }

        float directionX =
            target.position.x -
            transform.position.x;

        if (Mathf.Abs(directionX) <
            0.01f)
        {
            return;
        }

        // 기본 캐릭터가 왼쪽을 보는 기준
        spriteRenderer.flipX =
            directionX > 0f;
    }

    private void ResetAttackTimer()
    {
        if (enemyStats == null)
        {
            attackTimer = 0f;
            return;
        }

        // 공격속도 배율 적용
        attackTimer =
            enemyStats.AttackInterval /
            attackSpeedMultiplier;
    }
}
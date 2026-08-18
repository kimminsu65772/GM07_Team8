using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyStats))]
public class EnemyAnimationController : MonoBehaviour
{
    private static readonly int MoveParameter =
        Animator.StringToHash("1_Move");

    private static readonly int AttackParameter =
        Animator.StringToHash("2_Attack");

    private static readonly int DamagedParameter =
        Animator.StringToHash("3_Damaged");

    private static readonly int DeathParameter =
        Animator.StringToHash("4_Death");

    private static readonly int ChargeParameter =
        Animator.StringToHash("5_Other");

    [Header("SPUM Animator")]
    [SerializeField] private Animator enemyAnimator;

    [Header("Movement")]
    [SerializeField, Min(0f)]
    private float movementThreshold = 0.01f;

    private Rigidbody2D enemyRigidbody2D;
    private EnemyStats enemyStats;

    private void Awake()
    {
        enemyRigidbody2D =
            GetComponent<Rigidbody2D>();

        enemyStats =
            GetComponent<EnemyStats>();

        // 직접 연결하지 않았다면 자식 Animator를 자동으로 찾는다.
        if (enemyAnimator == null)
        {
            enemyAnimator =
                GetComponentInChildren<Animator>();
        }

        if (enemyAnimator == null)
        {
            Debug.LogError(
                $"{name}: 자식 오브젝트에서 SPUM Animator를 찾지 못했습니다."
            );

            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (enemyAnimator != null)
        {
            // 새로 생성된 적은 살아 있는 상태로 시작한다.
            enemyAnimator.SetBool(
                IsDeathParameter,
                false
            );
        }

        if (enemyStats == null)
        {
            return;
        }

        // EnemyStats가 알리는 피격·사망 이벤트를 구독한다.
        enemyStats.EnemyDamaged +=
            HandleEnemyDamaged;

        enemyStats.EnemyDied +=
            HandleEnemyDied;
    }

    private void Update()
    {
        if (enemyAnimator == null ||
            enemyStats == null)
        {
            return;
        }

        bool isMoving =
            !enemyStats.IsDead &&
            enemyRigidbody2D.linearVelocity.sqrMagnitude >
            movementThreshold *
            movementThreshold;

        enemyAnimator.SetBool(
            MoveParameter,
            isMoving
        );
    }

    // EnemyAttack의 Attack Event에서 호출한다.
    public void PlayAttack()
    {
        if (enemyAnimator == null ||
            enemyStats == null ||
            enemyStats.IsDead)
        {
            return;
        }

        enemyAnimator.SetTrigger(
            AttackParameter
        );
    }

    // 보스 몸통박치기 스킬에서 호출한다.
    public void PlayCharge()
    {
        if (enemyAnimator == null ||
            enemyStats == null ||
            enemyStats.IsDead)
        {
            return;
        }

        enemyAnimator.ResetTrigger(
            AttackParameter
        );

        enemyAnimator.SetBool(
            MoveParameter,
            false
        );

        enemyAnimator.SetTrigger(
            ChargeParameter
        );
    }

    private void HandleEnemyDamaged(
        EnemyStats damagedEnemy)
    {
        if (enemyAnimator == null)
        {
            return;
        }

        enemyAnimator.SetTrigger(
            DamagedParameter
        );
    }

    private void HandleEnemyDied(
        EnemyStats deadEnemy)
    {
        if (enemyAnimator == null)
        {
            return;
        }

        enemyAnimator.SetBool(
            MoveParameter,
            false
        );

        enemyAnimator.ResetTrigger(
            AttackParameter
        );

        enemyAnimator.ResetTrigger(
            DamagedParameter
        );

       

        // 먼저 사망 상태를 고정한 다음 사망 애니메이션을 실행한다.
        enemyAnimator.SetBool(
            IsDeathParameter,
            true
        );

        enemyAnimator.SetTrigger(
            DeathParameter
        );
    }

    // 사망 애니메이션 종료 후 IDLE로 돌아가지 않도록 유지한다.
    private static readonly int IsDeathParameter =
        Animator.StringToHash("isDeath");

    private void OnDisable()
    {
        if (enemyStats != null)
        {
            // 제거된 적의 이벤트 연결을 반드시 해제한다.
            enemyStats.EnemyDamaged -=
                HandleEnemyDamaged;

            enemyStats.EnemyDied -=
                HandleEnemyDied;
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool(
                MoveParameter,
                false
            );
        }
    }
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyStats))]
public class EnemyAnimationController : MonoBehaviour
{
    private static readonly int MoveParameter = Animator.StringToHash("1_Move");

    private static readonly int AttackParameter =  Animator.StringToHash("2_Attack");

    private static readonly int DamagedParameter =  Animator.StringToHash("3_Damaged");

    private static readonly int DeathParameter =  Animator.StringToHash("4_Death");

    private static readonly int OtherParameter5 = Animator.StringToHash("5_Other");

    private static readonly int OtherParameter6 =  Animator.StringToHash("6_Other");
    // 풀에서 재사용할 때 복구할 SPUM 내부 Transform 정보
    private Transform[] animatedTransforms;
    private Vector3[] initialLocalPositions;
    private Quaternion[] initialLocalRotations;
    private Vector3[] initialLocalScales;

    [Header("SPUM Animator")]
    [SerializeField] private Animator enemyAnimator;

    [Header("Movement")]
    [SerializeField, Min(0f)]
    private float movementThreshold = 0.01f;

    private Rigidbody2D enemyRigidbody2D;
    private EnemyStats enemyStats;

    private void Awake()
    {
        enemyRigidbody2D = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();

        // 직접 연결하지 않았다면 자식 Animator를 자동으로 찾는다.
        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponentInChildren<Animator>();
        }

        // Animator가 없으면 초기화를 중단한다.
        if (enemyAnimator == null)
        {
            Debug.LogError($"{name}: 자식 오브젝트에서 SPUM Animator를 찾지 못했습니다.");

            enabled = false;
            return;
        }

        // SPUM 내부 모든 자식 Transform을 가져온다.
        animatedTransforms =
            enemyAnimator.GetComponentsInChildren<Transform>(true);

        initialLocalPositions =
            new Vector3[animatedTransforms.Length];

        initialLocalRotations =
            new Quaternion[animatedTransforms.Length];

        initialLocalScales =
            new Vector3[animatedTransforms.Length];

        // 사망 애니메이션이 변경하기 전의 기본 Transform 값을 저장한다.
        for (int i = 0; i < animatedTransforms.Length; i++)
        {
            initialLocalPositions[i] =
                animatedTransforms[i].localPosition;

            initialLocalRotations[i] =
                animatedTransforms[i].localRotation;

            initialLocalScales[i] =
                animatedTransforms[i].localScale;
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

        enemyAnimator.SetBool(  MoveParameter, isMoving  );
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

        // 현재 캐릭터에 존재하는 Other 파라미터로 스킬을 실행한다.
        int otherParameter = GetOtherParameter();

        if (otherParameter != 0)
        {
            enemyAnimator.SetTrigger(otherParameter);
        }
    }
    // 광역 도트 스킬의 Other 애니메이션을 재생한다.
    public void PlayDotAreaSkill()
    {
        if (enemyAnimator == null || enemyStats == null || enemyStats.IsDead)
        {
            return;
        }

        enemyAnimator.ResetTrigger(AttackParameter);
        enemyAnimator.SetBool(MoveParameter, false);

        int otherParameter = GetOtherParameter();

        if (otherParameter != 0)
        {
            enemyAnimator.SetTrigger(otherParameter);
        }
    }
    private void HandleEnemyDamaged(
        EnemyStats damagedEnemy)
    {
        if (enemyAnimator == null)
        {
            return;
        }
        // 보스는 피격 모션을 재생하지 않는다.
        if (damagedEnemy.IsBoss)
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
    public void ResetForPool()
    {
        // 풀에서 다시 꺼낸 적의 애니메이션 상태를 초기화한다.
        enabled = true;

        if (enemyAnimator == null)
        {
            return;
        }

        // Animator Controller를 유지한 채 상태와 본래 포즈를 초기화한다.
        enemyAnimator.Rebind();

        // 사망 애니메이션이 변경한 SPUM 내부 Transform을 기본값으로 복구한다.
        if (animatedTransforms != null)
        {
            for (int i = 0; i < animatedTransforms.Length; i++)
            {
                if (animatedTransforms[i] == null)
                {
                    continue;
                }

                animatedTransforms[i].localPosition = initialLocalPositions[i];
                animatedTransforms[i].localRotation = initialLocalRotations[i];
                animatedTransforms[i].localScale = initialLocalScales[i];
            }
        }

        // 이전 사용에서 남은 공통 트리거를 제거한다.
        enemyAnimator.ResetTrigger(AttackParameter);
        enemyAnimator.ResetTrigger(DamagedParameter);
        enemyAnimator.ResetTrigger(DeathParameter);

        // 현재 Animator에 존재하는 Other 트리거만 초기화한다.
        int otherParameter = GetOtherParameter();

        if (otherParameter != 0)
        {
            enemyAnimator.ResetTrigger(otherParameter);
        }

        // 이동 및 사망 상태를 살아 있는 기본 상태로 변경한다.
        enemyAnimator.SetBool(MoveParameter, false);
        enemyAnimator.SetBool(IsDeathParameter, false);

        // 변경한 애니메이션 상태를 즉시 반영한다.
        enemyAnimator.Update(0f);
    }

    // 현재 Animator에 해당 파라미터가 있는지 확인한다.
    private bool HasAnimatorParameter(int parameterHash)
    {
        if (enemyAnimator == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in enemyAnimator.parameters)
        {
            if (parameter.nameHash == parameterHash)
            {
                return true;
            }
        }

        return false;
    }

    // 캐릭터 Animator에 맞는 Other 파라미터를 반환한다.
    private int GetOtherParameter()
    {
        if (HasAnimatorParameter(OtherParameter5))
        {
            return OtherParameter5;
        }

        if (HasAnimatorParameter(OtherParameter6))
        {
            return OtherParameter6;
        }

        return 0;
    }
}
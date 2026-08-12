using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyStats))]
public class EnemyAttack : MonoBehaviour
{
    private IDamageable targetDamageable;
    [Header("Target")]
    [SerializeField] private Transform target;
    [Header("Attack Range")]
    [SerializeField] private float heroAttackRange = 1.3f;
    [Header("Attack Event")]
    // 나중에 공격 애니메이션, 파티클, 사운드를 연결하기 위한 이벤트
    [SerializeField]
    private UnityEvent attackPerformed =
        new UnityEvent();

    private EnemyStats enemyStats;


    private float attackTimer;

    private void Awake()
    {
        // 같은 적 오브젝트의 능력치 정보를 가져온다.
        enemyStats = GetComponent<EnemyStats>();

        ResetAttackTimer();
    }

    private void Update()
    {
        // 타깃이 삭제됐거나 적이 죽었다면 공격을 중단한다.
        // 공격 도중 비행선이 파괴되는 경우를 처리하는 필수 예외 검사다.
        if (target == null ||
         targetDamageable == null ||
          enemyStats == null ||
          enemyStats.IsDead)
        {
            ResetAttackTimer();
            return;
        }
       float currentAttackRange;
         // 적이 비행선 오른쪽의 공격선에 도착했는지 X축 거리로 확인한다.
         // EnemyMovement의 정지 기준과 동일한 방식이다.
        
         float horizontalDistanceToTarget =
            Mathf.Abs(
                transform.position.x -
                target.position.x
            );
        // 현재 타깃이 Hero 레이어라면 근접 사거리 사용
        if (target.gameObject.layer ==
            LayerMask.NameToLayer("Hero"))
        {
            currentAttackRange = heroAttackRange;
        }
        else
        {
            // 비행선은 EnemyData의 기존 공격 사거리 사용
            currentAttackRange = enemyStats.AttackRange;
        }

        if (horizontalDistanceToTarget > currentAttackRange)
        {
            ResetAttackTimer();
            return;
        }
       
       

        // 공격 범위 밖이라면 공격 타이머를 초기화한다.
        if (horizontalDistanceToTarget >
            enemyStats.AttackRange)
        {
            ResetAttackTimer();
            return;
        }
        // 피격 경직 중에는 공격 타이머를 감소시키지 않는다.
        // 경직 시간만큼 다음 공격 시점이 자연스럽게 늦어진다.
        if (enemyStats.IsHitStunned)
        {
            return;
        }
        // 다음 공격까지 남은 시간을 감소시킨다.
        attackTimer -= Time.deltaTime;

        if (attackTimer > 0f)
        {
            return;
        }

        Attack();
        ResetAttackTimer();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        // 영웅 또는 비행선에서 IDamageable을 찾는다.
        targetDamageable = target != null
            ? target.GetComponentInParent<IDamageable>()
            : null;

        if (target != null && targetDamageable == null)
        {
            Debug.LogWarning(
                $"{name}: {target.name}에서 IDamageable을 찾지 못했습니다.");
        }

    ResetAttackTimer();
    }

    private void Attack()
    {
        // 실제 공격 직전에 타깃이 사라졌는지 다시 확인한다.
        if (target == null || targetDamageable == null)
        {
            return;
        }

        // EnemyData에 설정한 공격력만큼 테스트 비행선의 체력을 감소시킨다.
        targetDamageable.TakeDamage(
            enemyStats.AttackPower);

        // 공격 애니메이션, 파티클, 사운드용 이벤트
        attackPerformed?.Invoke();
    }

    private void ResetAttackTimer()
    {
        // EnemyData에 설정한 공격 간격으로 초기화한다.
        attackTimer = enemyStats != null
            ? enemyStats.AttackInterval
            : 0f;
    }
}
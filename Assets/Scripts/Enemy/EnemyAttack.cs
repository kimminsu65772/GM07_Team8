using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyStats))]
public class EnemyAttack : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Attack Event")]
    // 나중에 공격 애니메이션, 파티클, 사운드를 연결하기 위한 이벤트
    [SerializeField]
    private UnityEvent attackPerformed =
        new UnityEvent();

    private EnemyStats enemyStats;

    // 현재는 실제 비행선이 아닌 테스트용 체력 스크립트를 사용한다.
    private TestAirshipHealth targetHealth;

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
            targetHealth == null ||
            enemyStats == null ||
            enemyStats.IsDead)
        {
            ResetAttackTimer();
            return;
        }

        // 적이 비행선 오른쪽의 공격선에 도착했는지 X축 거리로 확인한다.
        // EnemyMovement의 정지 기준과 동일한 방식이다.
        float horizontalDistanceToTarget =
            Mathf.Abs(
                transform.position.x -
                target.position.x
            );

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
        // StageManager가 적을 생성한 뒤 호출하여 타깃을 지정한다.
        target = newTarget;

        // AirshipTarget 자신 또는 부모에 있는 테스트 체력을 찾는다.
        targetHealth = target != null
            ? target.GetComponentInParent<TestAirshipHealth>()
            : null;

        if (target != null && targetHealth == null)
        {
            Debug.LogWarning(
                $"{name}: 타깃에서 TestAirshipHealth를 찾지 못했습니다."
            );
        }

        ResetAttackTimer();
    }

    private void Attack()
    {
        // 실제 공격 직전에 타깃이 사라졌는지 다시 확인한다.
        if (target == null || targetHealth == null)
        {
            return;
        }

        // EnemyData에 설정한 공격력만큼 테스트 비행선의 체력을 감소시킨다.
        targetHealth.TakeDamage(
            enemyStats.AttackPower
        );

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
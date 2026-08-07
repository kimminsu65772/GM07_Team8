using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyStats))]
public class EnemyAttack : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Attack Event")]
    // 공격할 때 피해량을 전달하는 이벤트
    // 테스트에서는 TestAirshipHealth.TakeDamage와 연결한다.
    [SerializeField]
    private UnityEvent<int> attackPerformed =
        new UnityEvent<int>();

    private EnemyStats enemyStats;
    private float attackTimer;

    private void Awake()
    {
        // 같은 적 오브젝트에 붙어 있는 능력치 컴포넌트를 가져온다.
        enemyStats = GetComponent<EnemyStats>();

        ResetAttackTimer();
    }

    private void Update()
    {
        // 타깃이 파괴됐거나 적이 죽었다면 공격을 중단한다.
        // 공격 도중 타깃이 사라지는 상황을 처리하는 필수 예외 검사다.
        if (target == null ||
            enemyStats == null ||
            enemyStats.IsDead)
        {
            ResetAttackTimer();
            return;
        }

        float distanceToTarget = Vector2.Distance(
            transform.position,
            target.position);

        // 공격 범위 밖에 있으면 공격하지 않는다.
        if (distanceToTarget > enemyStats.AttackRange)
        {
            ResetAttackTimer();
            return;
        }

        // 매 프레임 경과 시간을 빼면서 다음 공격까지 기다린다.
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
        // 스테이지에서 적을 생성할 때 타깃을 지정할 수 있도록 만든 함수
        target = newTarget;
        ResetAttackTimer();
    }

    private void Attack()
    {
        // 공격 실행 직전에 타깃이 사라졌을 가능성을 한 번 더 검사한다.
        if (target == null)
        {
            return;
        }

        // EnemyData의 공격력을 이벤트에 연결된 대상에게 전달한다.
        attackPerformed?.Invoke(enemyStats.AttackPower);
    }

    private void ResetAttackTimer()
    {
        // EnemyData에 설정한 공격 간격으로 타이머를 초기화한다.
        attackTimer = enemyStats != null
            ? enemyStats.AttackInterval
            : 0f;
    }
}
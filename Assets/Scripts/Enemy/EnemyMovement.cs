using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyStats))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Enemy Avoidance")]
    [SerializeField, Min(0.1f)]
    private float avoidanceRadius = 1.2f;

    [SerializeField, Min(0f)]
    private float avoidanceStrength = 1.5f;

    private Rigidbody2D enemyRigidbody2D;
    private EnemyStats enemyStats;

    // 생성 순서에 따라 회피 방향을 위, 아래로 번갈아 배정한다.
    private static int nextAvoidanceOrder;
    private float avoidanceSide;

    private void Awake()
    {
        enemyRigidbody2D = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();

        // 첫 번째 적은 앞이 비어 있으므로 직진하고,
        // 뒤따라오는 적들은 필요할 때 서로 반대 방향으로 회피한다.
        avoidanceSide =
            nextAvoidanceOrder++ % 2 == 0 ? 1f : -1f;
    }

    private void FixedUpdate()
    {
        if (target == null ||
            enemyStats == null ||
            enemyStats.IsDead ||
             enemyStats.IsHitStunned)
        {
            StopMoving();
            return;
        }

        Vector2 targetPosition = target.position;
        Vector2 currentPosition = enemyRigidbody2D.position;

        // 적은 비행선 오른쪽에서 접근하므로
        // X축 거리가 공격 범위에 도달하면 더 왼쪽으로 이동하지 않는다.
        float horizontalDistanceToTarget =
            currentPosition.x - targetPosition.x;

        if (horizontalDistanceToTarget <= enemyStats.AttackRange)
        {
            // 물리 충돌로 선을 살짝 넘어간 경우에도
            // 공격 범위의 오른쪽 경계로 위치를 되돌린다.
            float limitPositionX =
                targetPosition.x + enemyStats.AttackRange;

            enemyRigidbody2D.position = new Vector2(
                limitPositionX,
                currentPosition.y
            );

            StopMoving();
            return;
        }

        Vector2 directDirection =
            (targetPosition - currentPosition).normalized;

        Vector2 moveDirection = CalculateMoveDirection(
            currentPosition,
            targetPosition,
            directDirection,
            horizontalDistanceToTarget
        );

        enemyRigidbody2D.linearVelocity =
            moveDirection * enemyStats.MoveSpeed;
    }

    private Vector2 CalculateMoveDirection(
        Vector2 currentPosition,
        Vector2 targetPosition,
        Vector2 directDirection,
        float myDistanceToTarget)
    {
        float verticalAvoidance = 0f;

        // 주변 Collider 중 EnemyMovement가 있는 적만 찾아낸다.
        Collider2D[] nearbyColliders =
            Physics2D.OverlapCircleAll(
                currentPosition,
                avoidanceRadius
            );

        foreach (Collider2D nearbyCollider in nearbyColliders)
        {
            EnemyMovement otherEnemy =
                nearbyCollider.GetComponentInParent<EnemyMovement>();

            if (otherEnemy == null ||
                otherEnemy == this ||
                otherEnemy.enemyStats == null ||
                otherEnemy.enemyStats.IsDead)
            {
                continue;
            }

            Vector2 otherPosition =
                otherEnemy.enemyRigidbody2D.position;

            float otherDistanceToTarget =
                Vector2.Distance(
                  otherPosition,
                  targetPosition
                  );

            // 나보다 비행선에 가까운 적만 장애물로 판단한다.
            if (otherDistanceToTarget >= myDistanceToTarget)
            {
                continue;
            }

            Vector2 directionToOther =
                otherPosition - currentPosition;

            float distanceToOther =
                directionToOther.magnitude;

            if (distanceToOther <= Mathf.Epsilon ||
                distanceToOther >= avoidanceRadius)
            {
                continue;
            }

            float forwardAmount = Vector2.Dot(
                directDirection,
                directionToOther.normalized
            );

            // 내 진행 방향 앞쪽에 있는 적만 회피한다.
            if (forwardAmount <= 0.3f)
            {
                continue;
            }

            // 가까울수록 위 또는 아래 방향으로 더 강하게 움직인다.
            float avoidanceWeight =
                1f - distanceToOther / avoidanceRadius;

            verticalAvoidance +=
                avoidanceSide *
                avoidanceStrength *
                avoidanceWeight;
        }

        Vector2 avoidanceDirection =
            Vector2.up * verticalAvoidance;

        return (directDirection + avoidanceDirection).normalized;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void StopMoving()
    {
        enemyRigidbody2D.linearVelocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        // Scene 창에서 적 감지 범위를 노란 원으로 표시한다.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            avoidanceRadius
        );
    }
}
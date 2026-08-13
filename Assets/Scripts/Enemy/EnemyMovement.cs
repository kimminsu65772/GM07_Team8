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

    [SerializeField]
    private float heroStopRange = 1.3f;

    private Rigidbody2D enemyRigidbody2D;
    private EnemyStats enemyStats;
    private Transform combatTarget;

    private static int nextAvoidanceOrder;
    private float avoidanceSide;

    // 생성됐을 때의 Y 위치
    private float moveLineY;

    private void Awake()
    {
        enemyRigidbody2D = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();

        moveLineY = transform.position.y;

        avoidanceSide =
            nextAvoidanceOrder++ % 2 == 0 ? 1f : -1f;
    }

    public void SetCombatTarget(Transform newTarget)
    {
        combatTarget = newTarget;
    }

    private void FixedUpdate()
    {
        if (combatTarget != null)
        {
            float combatDistance =
                Mathf.Abs(
                    enemyRigidbody2D.position.x -
                    combatTarget.position.x
                );

            bool isHero =
                combatTarget.gameObject.layer ==
                LayerMask.NameToLayer("Hero");

            float stopRange =
                isHero
                    ? heroStopRange
                    : enemyStats.AttackRange;

            if (combatDistance <= stopRange)
            {
                StopMoving();
                return;
            }
        }

        if (target == null ||
            enemyStats == null ||
            enemyStats.IsDead)
        {
            StopMoving();
            return;
        }

        Vector2 targetPosition = target.position;
        Vector2 currentPosition =
            enemyRigidbody2D.position;

        float horizontalDistanceToTarget =
            currentPosition.x -
            targetPosition.x;

        if (horizontalDistanceToTarget <=
            enemyStats.AttackRange)
        {
            float limitPositionX =
                targetPosition.x +
                enemyStats.AttackRange;

            enemyRigidbody2D.position =
                new Vector2(
                    limitPositionX,
                    currentPosition.y
                );

            StopMoving();
            return;
        }

        // 기본 이동은 X축으로만
        Vector2 directDirection =
            Vector2.left;

        Vector2 moveDirection =
            CalculateMoveDirection(
                currentPosition,
                targetPosition,
                directDirection,
                horizontalDistanceToTarget
            );

        // 원래 생성된 Y 위치로 천천히 돌아가게 함
        float lineDifference =
            moveLineY - currentPosition.y;

        moveDirection.y +=
            lineDifference * 0.8f;

        moveDirection.Normalize();

        enemyRigidbody2D.linearVelocity =
            moveDirection *
            enemyStats.MoveSpeed;
    }

    private Vector2 CalculateMoveDirection(
        Vector2 currentPosition,
        Vector2 targetPosition,
        Vector2 directDirection,
        float myDistanceToTarget)
    {
        float verticalAvoidance = 0f;

        Collider2D[] nearbyColliders =
            Physics2D.OverlapCircleAll(
                currentPosition,
                avoidanceRadius
            );

        foreach (Collider2D nearbyCollider
                 in nearbyColliders)
        {
            EnemyMovement otherEnemy =
                nearbyCollider
                    .GetComponentInParent<EnemyMovement>();

            if (otherEnemy == null ||
                otherEnemy == this ||
                otherEnemy.enemyStats == null ||
                otherEnemy.enemyStats.IsDead)
            {
                continue;
            }

            Vector2 otherPosition =
                otherEnemy
                    .enemyRigidbody2D.position;

            float otherDistanceToTarget =
                otherPosition.x -
                targetPosition.x;

            // 나보다 앞에 있는 적만 확인
            if (otherDistanceToTarget >=
                myDistanceToTarget)
            {
                continue;
            }

            Vector2 directionToOther =
                otherPosition -
                currentPosition;

            float distanceToOther =
                directionToOther.magnitude;

            if (distanceToOther <=
                    Mathf.Epsilon ||
                distanceToOther >=
                    avoidanceRadius)
            {
                continue;
            }

            float forwardAmount =
                Vector2.Dot(
                    directDirection,
                    directionToOther.normalized
                );

            if (forwardAmount <= 0.3f)
            {
                continue;
            }

            float avoidanceWeight =
                1f -
                distanceToOther /
                avoidanceRadius;

            verticalAvoidance +=
                avoidanceSide *
                avoidanceStrength *
                avoidanceWeight;
        }

        Vector2 avoidanceDirection =
            Vector2.up *
            verticalAvoidance;

        return (
            directDirection +
            avoidanceDirection
        ).normalized;
    }

    public void SetTarget(
        Transform newTarget)
    {
        target = newTarget;
    }

    private void StopMoving()
    {
        enemyRigidbody2D.linearVelocity =
            Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            avoidanceRadius
        );
    }
}
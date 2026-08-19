using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyStats))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [SerializeField]
    private float heroStopRange = 1.3f;

    private Rigidbody2D enemyRigidbody2D;
    private EnemyStats enemyStats;
    private Transform combatTarget;

    private float moveLineY;

    private void Awake()
    {
        enemyRigidbody2D =
            GetComponent<Rigidbody2D>();

        enemyStats =
            GetComponent<EnemyStats>();

        // 생성됐을 때의 Y 위치 저장
        moveLineY =
            transform.position.y;
    }

    public void SetCombatTarget(
        Transform newTarget)
    {
        combatTarget =
            newTarget;
    }

    private void FixedUpdate()
    {
        if (target == null ||
            enemyStats == null ||
            enemyStats.IsDead)
        {
            StopMoving();
            return;
        }

        Vector2 currentPosition =
            enemyRigidbody2D.position;

        Vector2 targetPosition =
            target.position;

        // 현재 공격 타깃이 있으면 거리 확인
        if (combatTarget != null)
        {
            float combatDistance =
                Mathf.Abs(
                    currentPosition.x -
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

        // 비행선 공격 범위보다 더 넘어가지 않게 처리
        float horizontalDistanceToTarget =
            currentPosition.x -
            targetPosition.x;

        if (horizontalDistanceToTarget <=
            enemyStats.AttackRange)
        {
            

            StopMoving();
            return;
        }

        Move();
    }

    private void Move()
    {
        // 생성된 Y 위치를 유지하면서 왼쪽으로 이동
        enemyRigidbody2D.linearVelocity =
            new Vector2(
                -enemyStats.MoveSpeed,
                0f
            );
    }

    public void SetTarget(
        Transform newTarget)
    {
        target =
            newTarget;
    }

    private void StopMoving()
    {
        enemyRigidbody2D.linearVelocity =
            Vector2.zero;
    }
}
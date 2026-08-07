using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyStats))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Rigidbody2D enemyRigidbody2D;
    private EnemyStats enemyStats;

    private void Awake()
    {
        enemyRigidbody2D = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();
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

        Vector2 targetPosition = target.position;
        Vector2 currentPosition = enemyRigidbody2D.position;

        float distanceToTarget =
            Vector2.Distance(currentPosition, targetPosition);

        if (distanceToTarget <= enemyStats.AttackRange)
        {
            StopMoving();
            return;
        }

        Vector2 direction =
            (targetPosition - currentPosition).normalized;

        enemyRigidbody2D.linearVelocity =
            direction * enemyStats.MoveSpeed;
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    private void StopMoving()
    {
        enemyRigidbody2D.linearVelocity = Vector2.zero;
    }
}
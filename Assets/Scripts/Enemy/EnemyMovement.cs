using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyStats))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    public Transform AirshipTarget => target;

    [Header("Stop Range")]
    [SerializeField] private float heroStopRange = 1.3f;
    [SerializeField] private float airshipStopRange = 1.5f;
    [SerializeField] private bool useAirshipStopRange;

    private Rigidbody2D enemyRigidbody2D;
    private EnemyStats enemyStats;
    private Transform combatTarget;

    private void Awake()
    {
        enemyRigidbody2D = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();
    }

    public void SetCombatTarget(Transform newTarget)
    {
        combatTarget = newTarget;
    }

    private void FixedUpdate()
    {
        if (target == null || enemyStats == null || enemyStats.IsDead)
        {
            StopMoving();
            return;
        }

        Vector2 currentPosition = enemyRigidbody2D.position;

        // 보스만 별도 비행선 정지 거리를 사용한다.
        float currentAirshipStopRange = useAirshipStopRange ? airshipStopRange : enemyStats.AttackRange;

        if (combatTarget != null)
        {
            float combatDistance = Mathf.Abs(currentPosition.x - combatTarget.position.x);
            bool isHero = combatTarget.gameObject.layer == LayerMask.NameToLayer("Hero");
            float stopRange = isHero ? heroStopRange : currentAirshipStopRange;

            if (combatDistance <= stopRange)
            {
                StopMoving();
                return;
            }
        }

        float airshipDistance = Mathf.Abs(currentPosition.x - target.position.x);

        if (airshipDistance <= currentAirshipStopRange)
        {
            StopMoving();
            return;
        }

        Move();
    }

    private void Move()
    {
        // Y축 위치는 유지하면서 비행선 방향으로 이동한다.
        enemyRigidbody2D.linearVelocity = new Vector2(-enemyStats.MoveSpeed, 0f);
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
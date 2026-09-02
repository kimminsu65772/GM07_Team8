using System.Collections.Generic;
using UnityEngine;

public enum EnemyStackGroup { None, Mob, Mob1, Mob2, RangedMob, RangedMob1, RangedMob2 }

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

    [Header("Enemy Stacking")]
    [SerializeField] private bool useEnemyStacking = true;
    [SerializeField] private EnemyStackGroup stackGroup = EnemyStackGroup.None;
    [SerializeField] private float stackingDistance = 0.5f;
    [SerializeField] private float stackingLineRange = 0.3f;

    private static readonly List<EnemyMovement> activeEnemies = new List<EnemyMovement>();

    private Rigidbody2D enemyRigidbody2D;
    private EnemyStats enemyStats;
    private Transform combatTarget;

    private void Awake()
    {
        enemyRigidbody2D = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();
    }
    private void OnEnable()
    {
        if (!activeEnemies.Contains(this))
        {
            activeEnemies.Add(this);
        }
    }

    private void OnDisable()
    {
        activeEnemies.Remove(this);
    }
    public void SetCombatTarget(Transform newTarget)
    {
        combatTarget = newTarget;
    }

    private void FixedUpdate()
    {
        if (target == null || enemyStats == null || enemyStats.IsDead || enemyStats.IsStunned)
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

        // 같은 그룹의 적이 같은 줄 앞쪽에서 멈춰 있으면 간격을 두고 대기한다.
        if (ShouldStopForEnemyAhead(currentPosition))
        {
            StopMoving();
            return;
        }

        Move();
    }

    private bool ShouldStopForEnemyAhead(Vector2 currentPosition)
    {
        if (!useEnemyStacking || stackGroup == EnemyStackGroup.None)
        {
            return false;
        }

        foreach (EnemyMovement otherEnemy in activeEnemies)
        {
            if (otherEnemy == this || !otherEnemy.gameObject.activeInHierarchy || otherEnemy.enemyStats == null || otherEnemy.enemyStats.IsDead)
            {
                continue;
            }

            if (!otherEnemy.useEnemyStacking || otherEnemy.stackGroup != stackGroup)
            {
                continue;
            }

            Vector2 otherPosition = otherEnemy.enemyRigidbody2D.position;
            float xDistance = currentPosition.x - otherPosition.x;
            float yDistance = Mathf.Abs(currentPosition.y - otherPosition.y);

            bool isEnemyAhead = xDistance > 0f;
            bool isSameLine = yDistance <= stackingLineRange;
            bool isWithinStackingDistance = xDistance <= stackingDistance;
            bool isEnemyStopped = Mathf.Abs(otherEnemy.enemyRigidbody2D.linearVelocity.x) < 0.01f;

            if (isEnemyAhead && isSameLine && isWithinStackingDistance && isEnemyStopped)
            {
                return true;
            }
        }

        return false;
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
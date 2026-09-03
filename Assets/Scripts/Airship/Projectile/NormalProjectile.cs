using UnityEngine;

public class NormalProjectile : AirshipProjectileBase
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] protected float projectileRadius = 0.2f;
    [SerializeField] protected bool drawOnlyWhenSelected = false;
    
    
    private float noTargetFlightDuration = 3f;
    private bool isNoTargetFlight;
    private float noTargetFlightTimer;

    private float combinedRadius;
    private Vector2 offset;
    
    
    private void OnEnable()
    {
        isNoTargetFlight = false;
        noTargetFlightTimer = 0f;
    }


    private void Update()
    {
        if (!isNoTargetFlight &&
            (target == null ||
             damageable == null ||
             (damageable is Hero hero && hero.IsDead) ||
             (damageable is EnemyStats enemy && enemy.IsDead)))
        {
            BeginNoTargetFlight();
        }

        if (isNoTargetFlight)
        {
            noTargetFlightTimer += Time.deltaTime;

            transform.position +=
                transform.right *
                moveSpeed *
                Time.deltaTime;

            if (TryHitEnemyDuringNoTargetFlight())
                return;

            if (noTargetFlightTimer >= noTargetFlightDuration)
            {
                ReturnToPool();
                return;
            }

            return;
        }

        Vector3 targetPosition = target.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        Vector2 direction =
            (Vector2)targetPosition -
            (Vector2)transform.position;

        if (direction.sqrMagnitude > 0f)
        {
            transform.right =
                direction.normalized;
        }

        combinedRadius =
            projectileRadius + targetRadius;

        offset =
            (Vector2)transform.position -
            (Vector2)targetPosition;

        if (offset.sqrMagnitude >
            combinedRadius * combinedRadius)
        {
            return;
        }

        OnHit();
        ReturnToPool();
    }
    
    private void BeginNoTargetFlight()
    {
        isNoTargetFlight = true;
        noTargetFlightTimer = 0f;

        target = null;
        damageable = null;
        targetRadius = 0f;
    }
    
    private bool TryHitEnemyDuringNoTargetFlight()
    {
        var enemies =
            BattleManager.Instance.StageManager?.TrackedEnemies;

        if (enemies == null)
            return false;

        Vector2 projectilePosition = transform.position;

        EnemyStats hitEnemy = null;
        float nearestSqrDistance = float.MaxValue;

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyStats enemy = enemies[i];

            if (enemy == null ||
                enemy.IsDead ||
                !enemy.gameObject.activeInHierarchy)
            {
                continue;
            }

            Vector2 enemyPosition =
                enemy.TargetPoint != null
                    ? enemy.TargetPoint.position
                    : enemy.transform.position;

            float combinedRadius =
                projectileRadius + enemy.HitRadius;

            float sqrDistance =
                (projectilePosition - enemyPosition).sqrMagnitude;

            if (sqrDistance >
                combinedRadius * combinedRadius)
            {
                continue;
            }

            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                hitEnemy = enemy;
            }
        }

        if (hitEnemy == null)
            return false;

        hitEnemy.TakeDamage(damageInfo);
        ReturnToPool();

        return true;
    }

    private void OnDrawGizmos()
    {
        if (drawOnlyWhenSelected)
            return;

        DrawGizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawOnlyWhenSelected)
            return;

        DrawGizmo();
    }

    private void DrawGizmo()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, projectileRadius);
    }
}

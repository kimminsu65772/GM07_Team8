using UnityEngine;

/// <summary>
/// 움직임 로직을 위한 적 감지 기능.
/// </summary>
public class AirshipEnemyChecker : MonoBehaviour
{
    [Header("Slow Stop Box")]
    [SerializeField] private float width = 15f;
    [SerializeField] private float height = 10f;
    
    [Header("Immediate Stop Box")]
    [SerializeField] private float immediateStopWidth = 10f;
    [SerializeField] private float immediateStopHeight = 10f;
    
    [Header("Attack Target Box")]
    [SerializeField] private float attackTargetWidth = 17f;
    [SerializeField] private float attackTargetHeight = 10f;
    
    [SerializeField] private Vector2 pivotOffset;
    [SerializeField] private LayerMask enemyLayer;

    public bool HasEnemy()
    {
        Collider2D hit = Physics2D.OverlapBox(
            GetBoxCenter(width),
            GetBoxSize(width, height),
            0f,
            enemyLayer
        );

        return hit != null;
    }

    public bool HasImmediateStopEnemy()
    {
        Collider2D hit = Physics2D.OverlapBox(
            GetBoxCenter(immediateStopWidth),
            GetBoxSize(immediateStopWidth, immediateStopHeight),
            0f,
            enemyLayer
        );

        return hit != null;
    }
    
    public EnemyStats FindNearestEnemy()
    {
        var enemies =
            BattleManager.Instance.StageManager?.TrackedEnemies;

        if (enemies == null)
            return null;

        Vector2 boxCenter =
            GetBoxCenter(attackTargetWidth);

        Vector2 halfSize =
            GetBoxSize(
                attackTargetWidth,
                attackTargetHeight
            ) * 0.5f;

        EnemyStats nearestEnemy = null;
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
                enemy.transform.position;

            bool outsideBox =
                Mathf.Abs(enemyPosition.x - boxCenter.x) >
                halfSize.x ||
                Mathf.Abs(enemyPosition.y - boxCenter.y) >
                halfSize.y;

            if (outsideBox)
                continue;

            float sqrDistance =
                ((Vector2)enemy.transform.position -
                 (Vector2)transform.position).sqrMagnitude;

            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private Vector2 GetBoxPivot()
    {
        return (Vector2)transform.position + pivotOffset;
    }

    private Vector2 GetBoxCenter(float boxWidth)
    {
        return GetBoxPivot() + Vector2.right * (boxWidth * 0.5f);
    }

    private Vector2 GetBoxSize(float boxWidth, float boxHeight)
    {
        return new Vector2(boxWidth, boxHeight);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = HasEnemy() ? Color.red : Color.green;
        Gizmos.DrawWireCube(GetBoxCenter(width), GetBoxSize(width, height));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            GetBoxCenter(immediateStopWidth),
            GetBoxSize(immediateStopWidth, immediateStopHeight)
        );
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(
            GetBoxCenter(attackTargetWidth),
            GetBoxSize(attackTargetWidth, attackTargetHeight)
        );
    }
}
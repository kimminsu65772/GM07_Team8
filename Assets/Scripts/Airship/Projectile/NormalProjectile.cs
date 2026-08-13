using UnityEngine;

public class NormalProjectile : AirshipProjectileBase
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] protected float projectileRadius = 0.2f;
    [SerializeField] protected bool drawOnlyWhenSelected = false;

    private float combinedRadius;
    private Vector2 offset;

    private void Update()
    {
        if (target == null || damageable == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );
        Vector2 direction =
            (Vector2)target.position - (Vector2)transform.position;

        if (direction.sqrMagnitude > 0f)
        {
            transform.right = direction.normalized;
        }
        
        combinedRadius = projectileRadius + targetRadius;
        offset = (Vector2)transform.position - (Vector2)target.position;

        if (offset.sqrMagnitude > combinedRadius * combinedRadius)
            return;

        OnHit();
        Destroy(gameObject);
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

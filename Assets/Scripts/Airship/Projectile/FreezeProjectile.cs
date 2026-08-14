using System.Collections.Generic;
using UnityEngine;

public class FreezeProjectile : NormalProjectile
{
    [Header("Splash")]
    [SerializeField]
    private float splashRadius = 1.5f;
    [SerializeField]
    private float splashDamageMultiplier = 0.5f;

    [SerializeField]
    private LayerMask enemyLayer;

    [Header("VFX")]
    [SerializeField]
    private GameObject impactVfx;

    protected override void OnHit()
    {
        base.OnHit();

        ApplyFreezeStatus(target);
        ApplySplashDamage(transform.position);
        SpawnImpactVfx(transform.position);
    }

    private void ApplyFreezeStatus(Transform hitTarget)
    {
        if (hitTarget == null)
            return;

        // TODO: 적 상태이상 시스템 구현 후 빙결 버프 적용
    }

    private void ApplySplashDamage(Vector2 center)
    {
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                center,
                splashRadius,
                enemyLayer
            );

        HashSet<IDamageable> damagedTargets =
            new HashSet<IDamageable>();

        foreach (Collider2D hit in hits)
        {
            IDamageable targetDamageable =
                hit.GetComponentInParent<IDamageable>();

            if (targetDamageable == null ||
                targetDamageable == damageable ||
                !damagedTargets.Add(targetDamageable))
            {
                continue;
            }

            targetDamageable.TakeDamage(
                new DamageInfo(
                    damageInfo.Damage * splashDamageMultiplier,
                    damageInfo.IsCritical)
            );
        }
    }

    private void SpawnImpactVfx(Vector2 position)
    {
        if (impactVfx == null)
            return;

        GameObject vfx = Instantiate(
            impactVfx,
            position,
            Quaternion.identity
        );
        vfx.transform.localScale = Vector3.one * 1.34f;
    }
    
    private void OnDrawGizmos()
    {
        if (drawOnlyWhenSelected)
            return;

        DrawSplashGizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawOnlyWhenSelected)
            return;

        DrawSplashGizmo();
    }

    private void DrawSplashGizmo()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, projectileRadius);
        
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            splashRadius
        );
    }
}
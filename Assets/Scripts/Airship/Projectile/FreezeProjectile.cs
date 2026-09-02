using System.Collections.Generic;
using UnityEngine;

public class FreezeProjectile : NormalProjectile
{
    [SerializeField] private float stunDuration = 2f;
    [Header("Splash")]
    [SerializeField]
    private float splashRadius = 1.5f;
    [SerializeField]
    private float splashDamageMultiplier = 0.5f;

    [SerializeField]
    private LayerMask enemyLayer;

    [SerializeField]
    private AudioClip explodeSfxClip;
    [SerializeField, Range(0f, 1f)]
    private float explodeSfxVolume = 1f;

    protected override void OnHit()
    {
        base.OnHit();

        ApplyFreezeStatus(damageable);
        ApplySplashDamage(transform.position);
        SpawnImpactVfx(transform.position);
    }

    private void ApplyFreezeStatus(IDamageable targetDamageable)
    {
        if (targetDamageable == null)
            return;

        targetDamageable.Stun(stunDuration);
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
            ApplyFreezeStatus(targetDamageable);
        }
    }

    private void SpawnImpactVfx(Vector2 position)
    {
        if (PoolingManager.Instance == null)
        {
            return;
        }

        GameObject vfx =
            PoolingManager.Instance.GetFreezeImpactVfx(
                position,
                Quaternion.identity
            );

        if (vfx == null)
        {
            return;
        }

        vfx.transform.localScale =
            Vector3.one * 1.34f;
        
        if (explodeSfxClip != null)
        {
            SoundManager.Instance.PlaySound(
                explodeSfxClip,
                explodeSfxVolume
            );
        }
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

    // 스플래시 vfx 크기 설정하기 위한 용도
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
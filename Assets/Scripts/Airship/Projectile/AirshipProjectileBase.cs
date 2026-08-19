using UnityEngine;

public abstract class AirshipProjectileBase : MonoBehaviour
{
    protected Transform target;
    protected IDamageable damageable;
    protected DamageInfo damageInfo;
    protected float targetRadius;
    protected float targetHeightOffset;
    
    public virtual void Init(
        Transform target,
        IDamageable damageable,
        DamageInfo damageInfo,
        float targetHeightOffset = 0f)
    {
        this.target = target;
        this.damageable = damageable;
        this.damageInfo = damageInfo;

        targetRadius = damageable.HitRadius;
        this.targetHeightOffset = targetHeightOffset;
    }
    protected virtual void OnHit()
    {
        damageable.TakeDamage(damageInfo);
    }
}

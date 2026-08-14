using UnityEngine;

public abstract class AirshipProjectileBase : MonoBehaviour
{
    protected Transform target;
    protected IDamageable damageable;
    protected DamageInfo damageInfo;
    protected float targetRadius;

    public virtual void Init(
        Transform target,
        IDamageable damageable,
        DamageInfo damageInfo)
    {
        this.target = target;
        this.damageable = damageable;
        this.damageInfo = damageInfo;

        targetRadius = damageable.HitRadius;
    }
    protected virtual void OnHit()
    {
        damageable.TakeDamage(damageInfo);
    }
}

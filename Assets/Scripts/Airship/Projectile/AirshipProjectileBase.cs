using UnityEngine;

public abstract class AirshipProjectileBase : MonoBehaviour
{
    protected Transform target;
    protected IDamageable damageable;
    protected float damage;
    protected float targetRadius;

    public virtual void Init(
        Transform target,
        IDamageable damageable,
        float damage)
    {
        this.target = target;
        this.damageable = damageable;
        this.damage = damage;

        targetRadius = damageable.HitRadius;
    }
}

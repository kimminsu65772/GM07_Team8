using UnityEngine;

public abstract class AirshipProjectileBase : MonoBehaviour
{
    protected Transform target;
    protected IDamageable damageable;
    protected DamageInfo damageInfo;
    protected float targetRadius;

    private PoolingManager poolingManager;
    private AirshipCannonType poolingType;

    public virtual void Init(
        Vector3 startPosition,
        Quaternion startRotation,
        Transform target,
        IDamageable damageable,
        DamageInfo damageInfo)
    {
        transform.SetPositionAndRotation(
            startPosition,
            startRotation
        );

        this.target = target;
        this.damageable = damageable;
        this.damageInfo = damageInfo;

        targetRadius = damageable.HitRadius;

        gameObject.SetActive(true);
    }

    protected virtual void OnHit()
    {
        damageable.TakeDamage(damageInfo);
    }

    public void SetPoolingManager(
        PoolingManager poolingManager,
        AirshipCannonType poolingType)
    {
        this.poolingManager = poolingManager;
        this.poolingType = poolingType;
    }

    protected void ReturnToPool()
    {
        if (poolingManager == null)
        {

            gameObject.SetActive(false);
            return;
        }

        poolingManager.ReleaseAirshipProjectile(
            this,
            poolingType
        );
    }
}
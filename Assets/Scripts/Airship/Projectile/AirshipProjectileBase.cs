using UnityEngine;

public abstract class AirshipProjectileBase : MonoBehaviour
{
    protected Transform target;
    protected IDamageable damageable;
    protected DamageInfo damageInfo;
    protected float targetRadius;
    protected float targetHeightOffset;

    private PoolingManager poolingManager;
    private AirshipCannonType poolingType;

    public virtual void Init(
        Vector3 startPosition,
        Quaternion startRotation,
        Transform target,
        IDamageable damageable,
        DamageInfo damageInfo,
        float targetHeightOffset = 0f)
    {
        transform.SetPositionAndRotation(
            startPosition,
            startRotation
        );

        this.target = target;
        this.damageable = damageable;
        this.damageInfo = damageInfo;

        targetRadius = damageable.HitRadius;
        this.targetHeightOffset = targetHeightOffset;

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
            Debug.LogError(
                "투사체에 PoolingManager가 연결되지 않았습니다.",
                this
            );

            gameObject.SetActive(false);
            return;
        }

        poolingManager.ReleaseAirshipProjectile(
            this,
            poolingType
        );
    }
}
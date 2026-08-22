using UnityEngine;

public class HealProjectile : NormalProjectile
{
    protected override void OnHit()
    {
        if (damageable == null)
        {
            return;
        }

        damageable.Heal(damageInfo);
    }
}

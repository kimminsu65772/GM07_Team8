
using UnityEngine;

public class ArcherSkillProjectile : HeroAttackProjectileController
{
    private bool isExploded;

    [Header("Target Effect Preset")]
    [SerializeField] protected Vector2 targetPosPreset = new Vector2(0f, 0f);
    [SerializeField] protected Vector2 targetScalePreset = new Vector2(5f, 5f);

    private void OnEnable()
    {
        isExploded = false;
    }

    protected override void Update()
    {
        if (isExploded) return;

        base.Update();
    }
    
    protected override void OnHitTarget(IDamageable enemy)
    {
        Explode();
        if (projectileAudio != null) SoundManager.Instance.PlaySound(projectileAudio.clip, 1f);
    }

    protected override void OnReachTargetPosition()
    {
        Explode();
    }

    private void Explode()
    {
        if (isExploded)
            return;

        isExploded = true;

        transform.position = targetPosition;

        if (hero != null)
        {
            if (hero.Attack != null)
            {
                hero.Attack.AreaAttack(0, transform, 4f, 1.8f);
            }

            if (hero.Attack != null && hero.Attack.VFX != null)
            {
                hero.Attack.VFX.PlayTargetEffect(transform, targetPosPreset, targetScalePreset);
            }
        }

        ReturnToPool();
    }
}
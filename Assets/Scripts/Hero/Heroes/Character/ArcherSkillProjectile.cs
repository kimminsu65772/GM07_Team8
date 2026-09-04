
using System.Collections;
using UnityEngine;

public class ArcherSkillProjectile
    : HeroAttackProjectileController
{
    [SerializeField] private EffectPlayer vfx;

    private Vector2 posPreset =
        new Vector2(0f, 0.8f);

    private Vector2 scalePreset =
        new Vector2(5f, 5f);

    private Hero hero;

    private bool isExploded;

    private Coroutine effectCoroutine;

    private void OnEnable()
    {
        isExploded = false;

        effectCoroutine =
            StartCoroutine(PlayEffectLoop());
    }

    private void OnDisable()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
            effectCoroutine = null;
        }
    }

    protected override void Update()
    {
        if (isExploded) return;

        base.Update();
    }
    
    protected override void OnHitTarget(IDamageable enemy)
    {
        Explode();
    }

    protected override void OnReachTargetPosition()
    {
        Explode();
    }

    private IEnumerator PlayEffectLoop()
    {
        while (true)
        {
            if (vfx != null)
            {
                vfx.PlayAttackEffect(Vector2.zero, new Vector2(0.7f, 0.7f), transform.eulerAngles +
                    new Vector3(0f, 0f, 180f));
            }

            yield return new WaitForSeconds(0.05f * 6);
        }
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
                hero.Attack.VFX.PlayTargetEffect(transform, posPreset, scalePreset);
            }
        }

        ReturnToPool();
    }
}
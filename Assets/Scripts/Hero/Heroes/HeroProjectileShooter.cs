using UnityEngine;

public class HeroProjectileShooter : MonoBehaviour
{
    [SerializeField] private Hero hero;

    private HeroAttack heroAttack;

    private void Awake()
    {
        if (hero == null) hero = GetComponentInParent<Hero>();

        if (hero != null) heroAttack = hero.GetComponent<HeroAttack>();
    }

    public void FireProjectile()
    {
        if (hero == null || heroAttack == null) return;

        GameObject targetEnemy = hero.TargetEnemy;

        if (targetEnemy == null || !targetEnemy.activeSelf) return;

        if (targetEnemy.TryGetComponent<EnemyStats>(out EnemyStats enemy))
        {
            if (enemy.IsDead) return;
        }

        double damage = hero.HeroAtk;
        float criRan = Random.Range(1f, 100f);
        bool isCrit = false;

        if (criRan <= hero.HeroCriChance)
        {
            damage *= 2f;
            isCrit = true;
        }

        heroAttack.ThrowProjectile(targetEnemy.transform, new DamageInfo(damage, isCrit)
        );
    }
}
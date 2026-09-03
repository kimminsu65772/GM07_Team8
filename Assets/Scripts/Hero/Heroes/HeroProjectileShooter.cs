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
        if (hero == null || heroAttack == null || hero.TargetEnemy == null) return;

        Transform targetEnemy = hero.TargetEnemy.GetComponent<EnemyStats>().TargetPoint;

        if (targetEnemy == null || !targetEnemy.gameObject.activeSelf) return;

        if (targetEnemy.TryGetComponent<EnemyStats>(out EnemyStats enemy))
        {
            if (enemy.IsDead) return;
        }

        heroAttack.ThrowProjectile(targetEnemy.transform);
    }
}
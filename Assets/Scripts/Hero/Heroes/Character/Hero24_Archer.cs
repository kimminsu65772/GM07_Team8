using UnityEngine;

public class Hero24_Archer : Hero
{
    [SerializeField] private HeroProjectileType projectileType;
    [SerializeField] private Transform firePoint;

    protected override void Awake()
    {
        statTable = new Hero24StatTable();

        SetAttackEffectPreset(0f, 0f, 1f, 1f);
        SetSkillEffectPreset(0f, 0f, 1f, 1f);
        SetTargetEffectPreset(0f, 0f, 1f, 1f);

        Init(24, 1.1f, 6f, HeroLocationEnum.Back);
    }

    public override void Skill(GameObject enemy)
    {
        if (enemy == null || IsDead) return;
        if (firePoint == null) return;

        HeroAttackProjectileController projectile = PoolingManager.Instance.GetHeroProjectile(projectileType);

        if (projectile == null) return;

        ArcherSkillProjectile skillArrow = projectile.GetComponent<ArcherSkillProjectile>();

        if (skillArrow == null) return;

        projectile.transform.position = firePoint.position;
        skillArrow.Init(this, enemy);
    }
}
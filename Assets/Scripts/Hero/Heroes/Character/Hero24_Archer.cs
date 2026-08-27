using UnityEngine;

public class Hero24_Archer : Hero
{
    [SerializeField] private ArcherSkillProjectile skillArrowPrefab;

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
        if (skillArrowPrefab == null) return;

        ArcherSkillProjectile skillArrow = Instantiate(skillArrowPrefab, transform.position, Quaternion.identity);
        skillArrow.Init(this, enemy);
    }
}

using UnityEngine;

public class Hero13_Shieldman : Hero
{
    protected override void Awake()
    {
        statTable = new Hero13StatTable();
        SetAttackEffectPreset(-0.3f, 0.4f, -1.5f, 1.5f);
        SetSkillEffectPreset(-0.3f, 0.4f, -1.7f, 1.7f);
        SetTargetEffectPreset(0f, 0f, 1.5f, 1.5f);
        Init(13, 3f, 8f, HeroLocationEnum.Front);
    }

    public override void Skill(GameObject enemy)
    {
        // 데미지 적용
        float criRan = Random.Range(1f, 100f);
        double damage = HeroAtk * 1.2f;

        Vector2 direction = enemy.transform.position - transform.position;
        FlipSprite(direction);

        Attack.VFX.PlayTargetEffect(enemy.transform, TargetPosPreset, TargetScalePreset);

        bool isCrit = false;
        if (criRan <= HeroCriChance)
        {
            damage *= 2f;
            isCrit = true;
        }

        if (enemy.TryGetComponent<IDamageable>(out IDamageable enemyHP))
        {
            enemyHP.TakeDamage(new DamageInfo(damage, isCrit));
        }

        // 기절 적용
        float stunDuration = 3f;
        EnemyStats enemyStat = enemy.GetComponent<EnemyStats>();
        if (enemyStat == null) return;

        if (!enemyStat.IsBoss) enemyStat.Stun(stunDuration);
        else enemyStat.Stun(stunDuration / 2);
    }
}

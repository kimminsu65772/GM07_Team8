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
        Vector2 direction = enemy.transform.position - transform.position;
        FlipSprite(direction);

        Attack.VFX.PlayTargetEffect(enemy.transform, TargetPosPreset, TargetScalePreset);

        if (enemy.TryGetComponent<IDamageable>(out IDamageable enemyHP))
        {
            enemyHP.TakeDamage(Attack.GetDamageInfo(1.2));
        }

        // 기절 적용
        float stunDuration = 3f;
        EnemyStats enemyStat = enemy.GetComponent<EnemyStats>();
        if (enemyStat == null) return;

        if (!enemyStat.IsBoss) enemyStat.Stun(stunDuration);
        else enemyStat.Stun(stunDuration / 2);
    }
}

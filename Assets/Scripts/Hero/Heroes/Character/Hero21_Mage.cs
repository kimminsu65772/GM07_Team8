using UnityEngine;

public class Hero21_Mage : Hero
{
    protected override void Awake()
    {
        statTable = new Hero21StatTable();
        SetAttackEffectPreset(0f, 0.05f, 1.3f, 1.3f);
        SetSkillEffectPreset(0f, 0.05f, 1.5f, 1.7f);
        SetTargetEffectPreset(0f, 0.1f, 1.5f, 1.5f);
        Init(21, 1f, 6f, HeroLocationEnum.Back);
    }

    public override void Skill(GameObject enemy)
    {
        if (enemy == null || IsDead) return;

        Vector2 direction = enemy.transform.position - transform.position;
        FlipSprite(direction);

        Attack.VFX.PlayTargetEffect(enemy.transform, TargetPosPreset, TargetScalePreset);

        if (enemy.TryGetComponent<IDamageable>(out IDamageable enemyHP))
        {
            enemyHP.TakeDamage(Attack.GetDamageInfo(2));
        }
        // Debug.Log(gameObject.name + "의 스킬, 피해량 : " + damage);
    }
}
using UnityEngine;

public class Hero11_Warrior : Hero
{
    protected override void Awake()
    {
        statTable = new Hero11StatTable();
        SetAttackEffectPreset(-0.6f, 0.5f, -1.4f, 1.7f);
        SetSkillEffectPreset(-0.6f, 0.5f, 1.5f, 1.7f);
        Init(11, 2f, 5f, HeroLocationEnum.Front);
    }

    public override void Skill(GameObject enemy)
    {
        if (enemy == null || IsDead) return;

        Vector2 direction = enemy.transform.position - transform.position;
        FlipSprite(direction);
            

        if (enemy.TryGetComponent<IDamageable>(out IDamageable enemyHP))
        {
            enemyHP.TakeDamage(Attack.GetDamageInfo(1.5));
        }
    }
}

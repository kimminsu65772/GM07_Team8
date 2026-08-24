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
        float criRan = Random.Range(1f, 100f);
        double damage = HeroAtk * 1.5f;

        Vector2 direction = enemy.transform.position - transform.position;
        FlipSprite(direction);

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
        // Debug.Log(gameObject.name + "의 스킬, 피해량 : " + damage);
    }
}

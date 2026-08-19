using UnityEngine;

public class Cha1 : Hero
{
    protected override void Awake()
    {
        statTable = new Hero1StatTable();
        SetAttackEffectPreset(-0.6f, 0.5f, -1.4f, 1.7f);
        SetSkillEffectPreset(-0.6f, 0.5f, 1.5f, 1.7f);
        Init(1, 2f, 5f, HeroLocationEnum.Front);

        EditSkillText(
            "강타",
            "적을 세게 내리쳐 큰 피해를 입힙니다."
            );
    }

    public override void Skill(GameObject enemy)
    {
        float criRan = Random.Range(1f, 100f);
        float damage = HeroAtk * 1.5f;

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

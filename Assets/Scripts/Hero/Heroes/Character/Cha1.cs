using UnityEngine;

public class Cha1 : Hero
{
    protected override void Awake()
    {
        statTable = new Hero1StatTable();
        Init(-1, "Hero1", 2f, 5f, HeroLocationEnum.Front);
    }

    public override void Skill(GameObject enemy)
    {
        float criRan = Random.Range(1f, 100f);
        float damage = HeroAtk * 1.5f;

        Vector2 direction = enemy.transform.position - transform.position;
        FlipSprite(direction);

        if (criRan <= HeroCriChance)
            damage *= 2f;

        if (enemy.TryGetComponent<IDamageable>(out IDamageable enemyHP))
        {
            enemyHP.TakeDamage(damage);
        }
        Debug.Log(gameObject.name + "의 스킬, 피해량 : " + damage);
    }
}

using UnityEngine;

public class Cha2 : Hero
{
    protected override void Awake()
    {
        statTable = new Hero2StatTable();
        Init(-2, "Hero2", 1f, 5f, HeroLocationEnum.Back);
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
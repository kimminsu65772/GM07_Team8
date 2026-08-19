using UnityEngine;

public class Cha2 : Hero
{
    protected override void Awake()
    {
        statTable = new Hero2StatTable();
        SetAttackEffectPreset(0f, 0.05f, 1.3f, 1.3f);
        SetSkillEffectPreset(0f, 0.05f, 1.5f, 1.7f);
        SetTargetEffectPreset(0f, 0.1f, 1.5f, 1.5f);
        Init(-2, 1f, 6f, HeroLocationEnum.Back);

        EditSkillText(
            "암석 찌르기",
            "커다란 돌을 상대 아래에 소환하여 큰 피해를 입힙니다."
            );
    }

    public override void Skill(GameObject enemy)
    {
        float criRan = Random.Range(1f, 100f);
        float damage = HeroAtk * 1.5f;

        Vector2 direction = enemy.transform.position - transform.position;
        FlipSprite(direction);

        attack.VFX.PlayTargetEffect(enemy.transform, TargetPosPreset, TargetScalePreset);

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
using UnityEngine;

public class HeroAttack : MonoBehaviour
{
    private Hero hero;
    private EffectPlayer vfx;

    private bool isAttacking;
    private bool isSkilling;
    private bool canAttack;

    private float attackTimer;
    private float skillTimer;

    [Header("원거리 공격 시")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firePoint;

    public EffectPlayer VFX => vfx;
    public bool IsAttacking => isAttacking;
    public bool IsSkilling => isSkilling;
    public bool CanAttack => canAttack;
    public float SkillTimer => skillTimer;

    private void Awake()
    {
        hero = GetComponent<Hero>();
        vfx = GetComponentInChildren<EffectPlayer>();

        attackTimer = hero.HeroAttackTime;
        skillTimer = hero.HeroSkillTime;

        isAttacking = false;
        isSkilling = false;
    }

    private void Update()
    {
        if (attackTimer < hero.HeroAttackTime)
            attackTimer += Time.deltaTime;

        if (skillTimer < hero.HeroSkillTime)
            skillTimer += Time.deltaTime;
    }

    public void MeleeAttack(GameObject enemy)
    {
        if (hero.Location != HeroLocationEnum.Front || hero.IsDead || isAttacking || isSkilling) return;

        if (attackTimer >= hero.HeroAttackTime)
        {
            float criRan = Random.Range(1f, 100f);
            float damage = hero.HeroAtk;

            isAttacking = true;
            attackTimer = 0f;

            Vector2 direction = enemy.transform.position - transform.position;
            hero.FlipSprite(direction);

            vfx.PlayAttackEffect(hero.AtkPosPreset, hero.AtkScalePreset);

            bool isCrit = false;
            if (criRan <= hero.HeroCriChance)
            {
                damage *= 2f;
                isCrit = true;
            }

            if (enemy.TryGetComponent<IDamageable>(out IDamageable enemyHP))
            {
                enemyHP.TakeDamage(new DamageInfo(damage, isCrit));
            }

            Debug.Log(gameObject.name + "의 공격, 피해량 : " + damage);
        }
    }

    public void RangeAttack()
    {
        if (hero.Location != HeroLocationEnum.Back || hero.IsDead || isAttacking || isSkilling) return;

        if (attackTimer >= hero.HeroAttackTime)
        {
            hero.SearchEnemy();
            if (hero.TargetEnemy == null) return;

            float criRan = Random.Range(1f, 100f);
            float damage = hero.HeroAtk;

            isAttacking = true;
            attackTimer = 0f;

            Vector2 direction = hero.TargetEnemy.transform.position - transform.position;
            hero.FlipSprite(direction);

            vfx.PlayAttackEffect(hero.AtkPosPreset, hero.AtkScalePreset);

            bool isCrit = false;
            if (criRan <= hero.HeroCriChance)
            {
                damage *= 2f;
                isCrit = true;
            }

            ThrowProjectile(hero.TargetEnemy.transform, new DamageInfo(damage, isCrit));

            Debug.Log(gameObject.name + "의 공격, 피해량 : " + damage);
        }
    }

    private void ThrowProjectile(Transform enemy, DamageInfo damageInfo)
    {
        GameObject projec = Instantiate(
            projectile,
            firePoint.position,
            Quaternion.identity
        );

        projec.GetComponent<HeroAttackProjectileController>()
            .Init(enemy, damageInfo);
    }

    public void UseSkill(GameObject enemy)
    {
        if (hero.IsDead || isAttacking || isSkilling) return;

        if (skillTimer >= hero.HeroSkillTime)
        {
            isSkilling = true;
            skillTimer = 0f;

            hero.Skill(enemy);
            vfx.PlaySkillEffect(hero.SkillPosPreset, hero.SkillScalePreset);
        }
    }

    public void StopIsAttacking()
    {
        isAttacking = false;
    }

    public void StopIsSkilling()
    {
        isSkilling = false;
    }

    public void ChangeCanAttack(bool value)
    {
        canAttack = value;
    }

    public void ClearCoolTime()
    {
        attackTimer = 0f;
        skillTimer = 0f;
    }
}
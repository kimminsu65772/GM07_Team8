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

    public bool IsAttacking => isAttacking;
    public bool IsSkilling => isSkilling;
    public bool CanAttack => canAttack;

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

        UseSkill();
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

            vfx.PlayAttackEffect();

            if (criRan <= hero.HeroCriChance)
                damage *= 2f;

            if (enemy.TryGetComponent<IDamageable>(out IDamageable enemyHP))
            {
                enemyHP.TakeDamage(damage);
            }

            Debug.Log(gameObject.name + "의 공격, 피해량 : " + damage);
        }
    }

    public void RangeAttack(GameObject enemy)
    {
        if (hero.Location != HeroLocationEnum.Back || hero.IsDead || isAttacking || isSkilling) return;

        if (attackTimer >= hero.HeroAttackTime)
        {
            float criRan = Random.Range(1f, 100f);
            float damage = hero.HeroAtk;

            isAttacking = true;
            attackTimer = 0f;

            Vector2 direction = enemy.transform.position - transform.position;
            hero.FlipSprite(direction);

            vfx.PlayAttackEffect();

            if (criRan <= hero.HeroCriChance)
                damage *= 2f;

            ThrowProjectile(enemy.transform, damage);

            Debug.Log(gameObject.name + "의 공격, 피해량 : " + damage);
        }
    }

    private void ThrowProjectile(Transform enemy, float damage)
    {
        GameObject projec = Instantiate(
            projectile,
            firePoint.position,
            Quaternion.identity
        );

        projec.GetComponent<HeroAttackProjectileController>()
            .Init(enemy, damage);
    }

    public void UseSkill()
    {
        if (hero.IsDead || isAttacking || isSkilling) return;

        if (skillTimer >= hero.HeroSkillTime)
        {
            isSkilling = true;
            skillTimer = 0f;

            hero.Skill();
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
}
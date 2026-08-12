using UnityEngine;

public class HeroAttack : MonoBehaviour
{
    private Hero hero;
    private EffectPlayer vfx;
    private bool isAttacking;
    private bool canAttack;
    private float attackTimer;

    [SerializeField] private GameObject projectile;
    
    public bool IsAttacking => isAttacking;
    public bool CanAttack => canAttack;

    private void Awake()
    {
        hero = GetComponent<Hero>();
        vfx = GetComponentInChildren<EffectPlayer>();
        attackTimer = hero.HeroAttackTime;
        isAttacking = false;
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;
    }

    public void MeleeAttack(GameObject enemy)
    {
        if (hero.Location != HeroLocationEnum.Front || hero.IsDead) return;

        if (attackTimer >= hero.HeroAttackTime && !isAttacking)
        {
            float criRan = Random.Range(1f, 100f);
            float damage = hero.HeroAtk; // 적 방어력 적용

            isAttacking = true;
            attackTimer = 0f;

            vfx.PlayAttackEffect();
            // SFX 적용

            if (criRan <= hero.HeroCriChance) damage *= 2f;

            // 공격 적용, 치명타 적용
            if (enemy.TryGetComponent<IDamageable>(out IDamageable enemyHP))
            {
                enemyHP.TakeDamage(damage);
            }
            Debug.Log(gameObject.name + "의 공격, 피해량 : " + damage);
        }
    }

    public void RangeAttack(GameObject enemy)
    {
        if (hero.Location != HeroLocationEnum.Back || hero.IsDead) return;

        if (attackTimer >= hero.HeroAttackTime && !isAttacking)
        {
            float criRan = Random.Range(1f, 100f);
            float damage = hero.HeroAtk; // 적 방어력 적용

            isAttacking = true;
            attackTimer = 0f;

            vfx.PlayAttackEffect();
            // SFX 적용

            if (criRan <= hero.HeroCriChance) damage *= 2f;

            // 공격 적용, 치명타 적용
            ThrowProjectile(enemy.transform, damage);
            Debug.Log(gameObject.name + "의 공격, 피해량 : " + damage);
        }
    }

    private void ThrowProjectile(Transform enemy, float damage)
    {
        GameObject projec = Instantiate(projectile, transform.position, Quaternion.identity);
        projec.GetComponent<HeroAttackProjectileController>().Init(enemy, damage);
    }

    public void StopIsAttacking()
    {
        isAttacking = false;
    }

    public void ChangeCanAttack(bool value)
    {
        canAttack = value;
    }
}

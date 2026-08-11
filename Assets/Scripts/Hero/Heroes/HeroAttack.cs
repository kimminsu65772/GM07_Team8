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
        if (attackTimer >= hero.HeroAttackTime && !isAttacking && hero.Formation == HeroLocationEnum.Front)
        {
            float criRan = Random.Range(1f, 100f);
            float damage = hero.HeroAtk; // 적 방어력 적용

            isAttacking = true;
            attackTimer = 0f;

            vfx.PlayAttackEffect();
            // SFX 적용

            if (criRan <= hero.HeroCriChance) damage *= 2f;

            // 공격 적용, 치명타 적용
            Debug.Log(gameObject.name + "의 공격, 피해량 : " + damage);
        }
    }

    public void RangeAttack(GameObject enemy)
    {
        float criRan = Random.Range(1f, 100f);
        float damage = hero.HeroAtk; // 적 방어력 적용

        isAttacking = true;
        attackTimer = 0f;

        vfx.PlayAttackEffect();
        // SFX 적용

        if (criRan <= hero.HeroCriChance) damage *= 2f;

        // 공격 적용, 치명타 적용
        Debug.Log(gameObject.name + "의 공격, 피해량 : " + damage);
    }

    private void ThrowProjectile(Transform enemy)
    {
        GameObject projec = Instantiate(projectile, transform.position, Quaternion.identity);
        
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

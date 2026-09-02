using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HeroAttack : MonoBehaviour
{
    private Hero hero;
    private EffectPlayer vfx;
    private HeroAnimationController aniCon;

    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isSkilling;
    [SerializeField] private bool canAttack;

    [SerializeField] private float attackTimer;
    [SerializeField] private float skillTimer;

    private bool isAutoSkill = true;
    
    [Header("원거리 공격 시")]
    [SerializeField] private HeroProjectileType projectileType;
    [SerializeField] private Transform firePoint;

    public EffectPlayer VFX => vfx;
    public bool IsAttacking => isAttacking;
    public bool IsSkilling => isSkilling;
    public bool CanAttack => canAttack;
    public float SkillTimer => skillTimer;
    public bool IsAutoSkill => isAutoSkill;

    private void Awake()
    {
        hero = GetComponent<Hero>();
        vfx = GetComponentInChildren<EffectPlayer>();
        aniCon = GetComponent<HeroAnimationController>();

        isAttacking = false;
        isSkilling = false;
    }

    private void Start()
    {
        attackTimer = hero.HeroAttackTime;
        skillTimer = hero.HeroSkillTime;
    }

    private void Update()
    {
        if (attackTimer < hero.HeroAttackTime) attackTimer += Time.deltaTime;
        if (skillTimer < hero.HeroSkillTime) skillTimer += Time.deltaTime;
        if (hero.TargetEnemy == null) return;

        if (hero.TargetEnemy.TryGetComponent<EnemyStats>(out EnemyStats enemy))
        {
            if (enemy.IsDead)
            {
                isAttacking = false;
                isSkilling = false;

                hero.SearchEnemy();
            }
        }
    }

    public void MeleeAttack()
    {
        if (hero.Location != HeroLocationEnum.Front || hero.IsDead || isAttacking || isSkilling) return;

        if (hero.TargetEnemy == null || !hero.TargetEnemy.activeSelf) hero.SearchEnemy();

        if (attackTimer >= hero.HeroAttackTime)
        {
            float criRan = Random.Range(1f, 100f);
            double damage = hero.HeroAtk;

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

            if (hero.TargetEnemy.TryGetComponent<IDamageable>(out IDamageable enemyHP))
            {
                enemyHP.TakeDamage(new DamageInfo(damage, isCrit));
            }

            aniCon.ResetPose();
            // Debug.Log(gameObject.name + "의 근접 공격, 피해량 : " + damage);
        }
    }

    public void RangeAttack()
    {
        if (hero.Location != HeroLocationEnum.Back || hero.IsDead || isAttacking || isSkilling) return;
        if (hero.TargetEnemy == null || !hero.TargetEnemy.activeSelf) hero.SearchEnemy();
        if (hero.TargetEnemy == null) return;

        if (attackTimer >= hero.HeroAttackTime)
        {
            hero.SearchEnemy();
            if (hero.TargetEnemy == null) return;

            isAttacking = true;
            attackTimer = 0f;

            Vector2 direction = hero.TargetEnemy.transform.position - transform.position;
            hero.FlipSprite(direction);

            vfx.PlayAttackEffect(hero.AtkPosPreset, hero.AtkScalePreset);

            // Debug.Log(gameObject.name + "의 원거리 공격, 피해량 : " + damage);
            if (hero is Hero23_RapidMage rapidMage && rapidMage.IsAdditionalAttackActive)
            {
                AttackAdditionalTarget();
            }
        }
    }

    private void AttackAdditionalTarget()
    {
        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(
                transform.position,
                hero.SearchRange,
                hero.EnemyLayer
            );

        Transform additionalTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D coll in enemies)
        {
            if (!coll.gameObject.activeSelf)
                continue;

            if (coll.TryGetComponent<EnemyStats>(
                out EnemyStats enemyStats))
            {
                if (enemyStats.IsDead)
                    continue;
            }

            // 현재 공격 중인 대상은 제외
            if (hero.TargetEnemy != null &&
                coll.gameObject == hero.TargetEnemy)
            {
                continue;
            }

            float distance =
                (coll.transform.position -
                 transform.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                additionalTarget = coll.transform;
            }
        }

        if (additionalTarget == null)
            return;

        float criRan = Random.Range(1f, 100f);
        double damage = hero.HeroAtk;

        bool isCrit = false;

        if (criRan <= hero.HeroCriChance)
        {
            damage *= 2f;
            isCrit = true;
        }

        // 기존 원거리 공격과 동일하게 투사체 발사
        ThrowProjectile(
            additionalTarget,
            new DamageInfo(damage, isCrit)
        );
    }

    // areaShape = 0(원), 1(사각형)
    public void AreaAttack(int areaShape, Transform target, float range, double damageBonus)
    {
        Collider2D[] enemies = null;

        switch(areaShape)
        {
            case 0:
                enemies = Physics2D.OverlapCircleAll(target.transform.position, range, hero.EnemyLayer);
                break;
            case 1:
                enemies = Physics2D.OverlapBoxAll(target.transform.position, new Vector2(range, range), 0f, hero.EnemyLayer);
                break;
            default:
                Debug.Log("잘못된 areaShape 값");
                break;
        }

        if (enemies == null)
        {
            Debug.Log("광역 공격 감지 실패");
            return;
        }
        Debug.Log($"광역 공격 감지 적 수: {enemies.Length}");

        Vector2 direction = target.position - transform.position;
        hero.FlipSprite(direction);

        vfx.PlayTargetEffect(target, hero.TargetPosPreset, hero.TargetScalePreset);

        foreach (Collider2D coll in enemies)
        {
            if (coll.gameObject.TryGetComponent<IDamageable>(out IDamageable enemyHP))
            {
                float criRan = Random.Range(1f, 100f);
                double damage = hero.HeroAtk * damageBonus;

                bool isCrit = false;
                if (criRan <= hero.HeroCriChance)
                {
                    damage *= 2f;
                    isCrit = true;
                }

                enemyHP.TakeDamage(new DamageInfo(damage, isCrit));
                Debug.Log($"{gameObject.name}의 광역 스킬, {coll.gameObject.name} 피해량 : {damage}");
            }
        }
    }

    public void ThrowProjectile(
        Transform enemy,
        DamageInfo damageInfo)
    {
        if (projectileType == HeroProjectileType.None ||
            firePoint == null ||
            PoolingManager.Instance == null)
        {
            return;
        }

        HeroAttackProjectileController projectile =
            PoolingManager.Instance.GetHeroProjectile(
                projectileType
            );

        if (projectile == null)
        {
            return;
        }

        projectile.Init(
            firePoint.position,
            Quaternion.identity,
            enemy,
            damageInfo
        );
    }

    public void UseSkill(GameObject enemy)
    {
        if (hero.IsDead || isAttacking || isSkilling) return;
        if (skillTimer < hero.HeroSkillTime) return;
        if (enemy == null) return;
        if (enemy.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
        {
            if (enemyStats.IsDead)
            {
                hero.SearchEnemy();
                enemy = hero.TargetEnemy;

                if (enemy == null) return;
            }
        }

        isSkilling = true;
        hero.Skill(enemy);
        vfx.PlaySkillEffect(hero.SkillPosPreset, hero.SkillScalePreset);
        skillTimer = 0f;
    }

    public void StopIsAttacking()
    {
        isAttacking = false;
        aniCon.ResetPose();
    }

    public void StopIsSkilling()
    {
        isSkilling = false;
        aniCon.ResetPose();
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

    public void SetAutoSkill(bool value)
    {
        isAutoSkill = value;
    }
}
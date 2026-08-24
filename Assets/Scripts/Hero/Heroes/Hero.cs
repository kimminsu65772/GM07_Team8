using System.Collections;
using UnityEngine;
using System;

public abstract class Hero : MonoBehaviour, IDamageable
{
    [Header("영웅 정보")]
    [SerializeField] private int heroID;
    [SerializeField] private string heroName;
    [SerializeField] private int heroLv;
    [SerializeField] private double heroMaxHP;
    [SerializeField] private double heroCurrentHP;
    [SerializeField] private double heroAtk;
    [SerializeField] private double heroDef;
    [SerializeField] private float heroCriChance;
    [SerializeField] private bool isDead;
    [SerializeField] private float hitRadius;
    [SerializeField] private float attackTime;
    [SerializeField] private float skillTime;

    private HeroLocationEnum location;
    protected IHeroStatTable statTable;

    [SerializeField] private Transform heroRoot;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private LayerMask enemyLayer;
    private const float PlacementFollowSpeedMultiplier = 1.2f;
    private GameObject targetEnemy;
    private Transform placementPoint;
    private AirshipMovement airshipMovement;
    private bool isMoving;
    
    private float searchRange = 12f;
    private float meleeRange = 1.3f;
    
    protected HeroStateEnum heroState;
    private HeroEquipmentManager heroEquip;
    protected HeroAttack attack;

    private bool canStun = true;
    public bool IsStunned { get; private set; }

    public Vector2 AtkPosPreset { get; private set; }
    public Vector2 AtkScalePreset {  get; private set; }
    public Vector2 SkillPosPreset { get; private set; }
    public Vector2 SkillScalePreset { get; private set; }
    public Vector2 TargetPosPreset { get; private set; }
    public Vector2 TargetScalePreset { get; private set; }

    public int HeroID => heroID;
    public string HeroName => heroName;
    public int HeroLv
    {
        get => heroLv;
        set => heroLv = Mathf.Max(1, value);
    }
    public double HeroMaxHP
    {
        get => heroMaxHP;
        set => heroMaxHP = Math.Max(0d, value);
    }
    public double HeroCurrentHP
    {
        get => heroCurrentHP;
        set => heroCurrentHP = Math.Clamp(value, 0d, HeroMaxHP);
    }
    public double HeroAtk
    {
        get => heroAtk;
        set => heroAtk = Math.Max(0d, value);
    }
    public double HeroDef
    {
        get => heroDef;
        set => heroDef = Math.Max(0d, value);
    }
    public float HeroCriChance
    {
        get => heroCriChance;
        set => heroCriChance = Mathf.Clamp(value, 0f, 100f);
    }
    public bool IsDead => isDead;
    public float HitRadius
    {
        get => hitRadius;
        set => hitRadius = Mathf.Max(0f, value);
    }
    public float HeroAttackTime
    {
        get => attackTime;
        set => attackTime = Mathf.Max(0f, value);
    }
    public float HeroSkillTime
    {
        get => skillTime;
        set => skillTime = Mathf.Max(0f, value);
    }

    public GameObject TargetEnemy => targetEnemy;
    public HeroLocationEnum Location => location;
    public HeroStateEnum HeroState => heroState;
    public LayerMask EnemyLayer => enemyLayer;
    public float HPRatio { get; private set; }

    protected virtual void Awake() { }
    public virtual void Skill(GameObject enemy) { }

    protected virtual void Init(int id, float attackTime, float skillTime,
        HeroLocationEnum location)
    {
        heroID = id;
        heroName = ((HeroNameEnum)id).ToString();
        //HeroSaveData heroData = PlayerInfo.Instance.TryGetHeroData(heroName, out heroData) ? heroData : null;
        //HeroLv = heroData.Level;
        HeroLv = 1;
        HeroLvManager.Instance.LvApply(HeroLv, this);
        heroCurrentHP = heroMaxHP;
        HeroAttackTime = attackTime;
        HeroSkillTime = skillTime;
        IsStunned = false;
        isDead = false;
        this.location = location;

        heroState = HeroStateEnum.Move;
        heroEquip = GetComponent<HeroEquipmentManager>();
        attack = GetComponent<HeroAttack>();

        FlipSprite(new Vector2(1, 0));
    }

    public void Initialize(
        Transform newPlacementPoint = null,
        AirshipMovement newAirshipMovement = null)
    {
        if (newPlacementPoint != null)
        {
            placementPoint = newPlacementPoint;
        }

        if (newAirshipMovement != null)
        {
            airshipMovement = newAirshipMovement;
        }

        HeroLvManager.Instance.LvApply(HeroLv, this);
        heroCurrentHP = heroMaxHP;
        IsStunned = false;
        isDead = false;
        heroState = HeroStateEnum.Idle;
        isMoving = false;
        targetEnemy = null;
        attack.ClearCoolTime();
    }

    public void Update()
    {
        HPRatio = (float)HeroCurrentHP / (float)HeroMaxHP;

        if (targetEnemy != null && !targetEnemy.activeSelf) targetEnemy = null;
        if (targetEnemy == null) SearchEnemy();
        if (targetEnemy == null && location == HeroLocationEnum.Front) MoveToPlacementPoint();
        if (targetEnemy != null && location == HeroLocationEnum.Front) MoveToEnemy();
        if (targetEnemy != null && location == HeroLocationEnum.Back)
        {
            if (attack.IsAutoSkill && skillTime <= attack.SkillTimer) attack.UseSkill(targetEnemy);
            else attack.RangeAttack();
        }

        ChangeState();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        double damage = damageInfo.Damage;
        
        // 방어력 적용
        double finalDamage = damage * 100d / (HeroDef + 100d);
        heroCurrentHP -= finalDamage;

        if (heroCurrentHP <= 0f)
        {
            StartCoroutine(DieAndRevive());
        }
    }

    public void Heal(DamageInfo damageInfo)
    {
        if (isDead || damageInfo.Damage <= 0f)
        {
            return;
        }

        // 나중에 데미지 텍스트랑 연결할때 이벤트로 넘겨주려면 실제 힐량이 필요
        double actualHeal =
            Math.Min(
                damageInfo.Damage,
                HeroMaxHP - HeroCurrentHP
            );

        if (actualHeal <= 0f)
        {
            return;
        }

        HeroCurrentHP += actualHeal;
    }

    public void SearchEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, searchRange, enemyLayer);
        Transform nearestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        if (nearestEnemy == null)
        {
            targetEnemy = null;
            return;
        }

        targetEnemy = nearestEnemy.gameObject;
    }

    private void MoveToEnemy()
    {
        if (location == HeroLocationEnum.Back || targetEnemy == null || IsDead || IsStunned)
        {
            isMoving = false;
            return;
        }

        Vector2 direction = targetEnemy.transform.position - transform.position;

        if (direction.sqrMagnitude <= meleeRange * meleeRange) attack.ChangeCanAttack(true);
        else attack.ChangeCanAttack(false);

        if (attack.CanAttack && location == HeroLocationEnum.Front)
        {
            isMoving = false;

            if (attack.IsAutoSkill && skillTime <= attack.SkillTimer) attack.UseSkill(targetEnemy);
            else attack.MeleeAttack(targetEnemy);
        }
        else
        {
            isMoving = true;
            FlipSprite(direction);

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetEnemy.transform.position,
                moveSpeed * Time.deltaTime);
        }
    }

    private void MoveToPlacementPoint()
    {
        if (placementPoint == null || airshipMovement == null || IsDead || IsStunned)
        {
            isMoving = false;
            return;
        }

        Vector3 targetPosition = placementPoint.position;

        if (targetPosition.x < transform.position.x)
        {
            isMoving = false;
            return;
        }

        float sqrDistance = (targetPosition - transform.position).sqrMagnitude;
        if (sqrDistance <= 0.0001f)
        {
            isMoving = false;
            return;
        }

        isMoving = true;
        float followSpeed = airshipMovement.CurrentMoveSpeed * PlacementFollowSpeedMultiplier;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime);

        FlipSprite(new Vector2(1, 0));
    }

    public void FlipSprite(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) < 0.01f || IsDead) return;

        Vector3 scale = heroRoot.localScale;
        scale.x = direction.x > 0 ? -1 : 1;
        heroRoot.localScale = scale;
    }


    protected IEnumerator DieAndRevive()
    {
        isDead = true;
        attack.StopIsAttacking();
        attack.StopIsSkilling();

        yield return new WaitForSeconds(3f);

        isDead = false;
        HeroCurrentHP = heroMaxHP;

        targetEnemy = null;
        SearchEnemy();
    }

    private void ChangeState()
    {
        if (isDead) heroState = HeroStateEnum.Die;
        else if (IsStunned) heroState = HeroStateEnum.Stunned;
        else if (attack.IsSkilling) heroState = HeroStateEnum.Skill;
        else if (attack.IsAttacking) heroState = HeroStateEnum.Attack;
        else if (isMoving) heroState = HeroStateEnum.Move;
        else heroState = HeroStateEnum.Idle;
    }

    public void AttackStop()
    {
        attack.StopIsAttacking();
    }

    public void SkillStop()
    {
        attack.StopIsSkilling();
    }

    public void EquipStatApply(Equipment equip)
    {
        HeroMaxHP += equip.BonusHP;
        HeroCurrentHP += equip.BonusHP;
        HeroDef += equip.BonusDef;
        HeroCriChance += equip.BonusCriChance;
    }

    public Equipment[] EquipInfo()
    {
        return new Equipment[]
        {
            heroEquip.CurrentWeaponEquip,
            heroEquip.CurrentBodyEquip,
            heroEquip.CurrentAccEquip
        };
    }

    public HeroStat GetStat(int lv)
    {
        return statTable.GetStat(lv);
    }

    public void SetAttackEffectPreset(float posX, float posY, float scaleX, float scaleY)
    {
        AtkPosPreset = new Vector2(posX, posY);
        AtkScalePreset = new Vector2(scaleX, scaleY);
    }
    public void SetSkillEffectPreset(float posX, float posY, float scaleX, float scaleY)
    {
        SkillPosPreset = new Vector2(posX, posY);
        SkillScalePreset = new Vector2(scaleX, scaleY);
    }
    public void SetTargetEffectPreset(float posX, float posY, float scaleX, float scaleY)
    {
        TargetPosPreset = new Vector2(posX, posY);
        TargetScalePreset = new Vector2(scaleX, scaleY);
    }

    public void Stun(float duration)
    {
        if (canStun) StartCoroutine(StunApply(duration));
    }

    private IEnumerator StunApply(float duration)
    {
        canStun = false;
        IsStunned = true;

        yield return new WaitForSeconds(duration);
        IsStunned = false;

        yield return new WaitForSeconds(duration * 2);
        canStun = true;
    }

    private void OnDrawGizmos()
    {
        // 근거리 공격 사거리
        Gizmos.color = Color.red;
        if (location == HeroLocationEnum.Back) Gizmos.DrawWireSphere(
            new Vector3(transform.position.x, transform.position.y, transform.position.z), searchRange);
    }
}

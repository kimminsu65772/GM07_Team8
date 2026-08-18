using System.Collections;
using TMPro;
using UnityEngine;

public abstract class Hero : MonoBehaviour, IDamageable
{
    [Header("영웅 정보")]
    [SerializeField] private int heroID;
    [SerializeField] private string heroName;
    [SerializeField] private int heroLv;
    [SerializeField] private float heroMaxHP;
    [SerializeField] private float heroCurrentHP;
    [SerializeField] private float heroAtk;
    [SerializeField] private float heroDef;
    [SerializeField] private float heroCriChance;
    [SerializeField] private bool isDead;
    [SerializeField] private float hitRadius;
    [SerializeField] private float attackTime;
    [SerializeField] private float skillTime;

    private HeroLocationEnum location;
    // private HeroAttackTypeEnum attackType;
    protected IHeroStatTable statTable;

    [SerializeField] private Transform heroRoot;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private LayerMask enemyLayer;
    private GameObject targetEnemy;
    private bool isMoving;
    
    private float searchRange = 50f;
    private float meleeRange = 1.3f;
    
    protected HeroStateEnum heroState;
    private HeroEquipmentManager heroEquip;
    protected HeroAttack attack;

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
    public float HeroMaxHP
    {
        get => heroMaxHP;
        set => heroMaxHP = Mathf.Max(0f, value);
    }
    public float HeroCurrentHP
    {
        get => heroCurrentHP;
        set => heroCurrentHP = Mathf.Clamp(value, 0f, HeroMaxHP);
    }
    public float HeroAtk
    {
        get => heroAtk;
        set => heroAtk = Mathf.Max(0f, value);
    }
    public float HeroDef
    {
        get => heroDef;
        set => heroDef = Mathf.Max(0f, value);
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

    protected virtual void Awake() { }
    public virtual void Skill(GameObject enemy) { }

    protected virtual void Init(int id, string name, float attackTime, float skillTime,
        HeroLocationEnum location)
    {
        heroID = id;
        heroName = name;
        //HeroSaveData heroData = PlayerInfo.Instance.TryGetHeroData(heroName, out heroData) ? heroData : null;
        //HeroLv = heroData.Level;
        HeroLv = 1;
        HeroLvManager.Instance.LvApply(HeroLv, this);
        heroCurrentHP = heroMaxHP;
        HeroAttackTime = attackTime;
        HeroSkillTime = skillTime;
        isDead = false;
        this.location = location;

        heroState = HeroStateEnum.Idle;
        heroEquip = GetComponent<HeroEquipmentManager>();
        attack = GetComponent<HeroAttack>();
    }

    public void Initialize()
    {
        HeroLvManager.Instance.LvApply(HeroLv, this);
        heroCurrentHP = heroMaxHP;
        isDead = false;
        heroState = HeroStateEnum.Idle;
        targetEnemy = null;
        attack.ClearCoolTime();
    }

    public void Update()
    {
        if (targetEnemy == null) SearchEnemy();
        if (targetEnemy != null && location == HeroLocationEnum.Front) MoveToEnemy();
        if (targetEnemy != null && location == HeroLocationEnum.Back)
        {
            if (skillTime <= attack.SkillTimer) attack.UseSkill(targetEnemy);
            else attack.RangeAttack();
        }

        ChangeState();

        // if (Input.GetKeyDown(KeyCode.A)) TakeDamage(10f);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        float damage = damageInfo.Damage;
        
        // 방어력 적용하기
        heroCurrentHP -= damage;

        if (heroCurrentHP <= 0f)
        {
            StartCoroutine(DieAndRevive());
        }
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
        Debug.Log($"{gameObject.name}의 목표 : {targetEnemy.name}");
    }

    private void MoveToEnemy()
    {
        if (location == HeroLocationEnum.Back || targetEnemy == null || IsDead)
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

            if (skillTime <= attack.SkillTimer) attack.UseSkill(targetEnemy);
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
        heroState = HeroStateEnum.Die;

        yield return new WaitForSeconds(3f);

        isDead = false;
        heroState = HeroStateEnum.Idle;
        HeroCurrentHP = heroMaxHP;
    }

    private void ChangeState()
    {
        if (isDead) heroState = HeroStateEnum.Die;
        else if (attack.IsSkilling) heroState = HeroStateEnum.Skill;
        else if (isMoving) heroState = HeroStateEnum.Move;
        else if (attack.IsAttacking) heroState = HeroStateEnum.Attack;
        else heroState = HeroStateEnum.Idle;
    }

    public void AttackStop()
    {
        heroState = HeroStateEnum.Idle;
        attack.StopIsAttacking();
    }

    public void SkillStop()
    {
        Debug.Log("스킬 중지");
        heroState = HeroStateEnum.Idle;
        attack.StopIsSkilling();
    }

    private void OnDrawGizmos()
    {
        // 근거리 공격 사거리
        Gizmos.color = Color.red;
        if (location == HeroLocationEnum.Front) Gizmos.DrawWireSphere(
            new Vector3(transform.position.x, transform.position.y, transform.position.z), meleeRange);
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
}

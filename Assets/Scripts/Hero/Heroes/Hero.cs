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

    private HeroLocationEnum location;
    // private HeroAttackTypeEnum attackType;
    protected IHeroStatTable statTable;

    [SerializeField] private TMP_Text name_T;

    [SerializeField] private Transform heroRoot;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private LayerMask enemyLayer;
    private GameObject targetEnemy;
    private bool isMoving;
    
    private float searchRange = 50f;
    private float meleeRange = 1.3f;
    
    protected HeroStateEnum heroState;
    private HeroEquipmentManager heroEquip;
    private HeroAttack attack;

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
    public HeroLocationEnum Location => location;
    public HeroStateEnum HeroState => heroState;

    protected virtual void Awake() { }
    protected virtual void Skill() { }

    protected virtual void Init(int id, string name, float attackTime,
        HeroLocationEnum location)
    {
        heroID = id;
        heroName = name;
        //HeroSaveData heroData = PlayerInfo.Instance.TryGetHeroData(heroName, out heroData) ? heroData : null;
        //HeroLv = heroData.Level;
        HeroLv = 1;
        HeroLvManager.Instance.LvApply(HeroLv, this);
        heroCurrentHP = heroMaxHP;
        isDead = false;
        this.location = location;

        if (name_T != null) name_T.text = heroName;

        heroState = HeroStateEnum.Idle;
        heroEquip = GetComponent<HeroEquipmentManager>();
        attack = GetComponent<HeroAttack>();
    }

    public void Update()
    {
        if (targetEnemy == null) SearchEnemy();
        if (targetEnemy != null && location == HeroLocationEnum.Front) MoveToEnemy();
        if (targetEnemy != null && location == HeroLocationEnum.Back) attack.RangeAttack(targetEnemy);

        ChangeState();

        if (Input.GetKeyDown(KeyCode.A)) TakeDamage(heroMaxHP);
    }

    public void TakeDamage(float damage)
    {
        // 방어력 적용하기
        heroCurrentHP -= damage;

        if (heroCurrentHP <= 0f)
        {
            StartCoroutine(DieAndRevive());
        }
    }

    private void SearchEnemy()
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
        if (location == HeroLocationEnum.Back || targetEnemy == null)
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
            attack.MeleeAttack(targetEnemy);
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

    private void FlipSprite(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) < 0.01f) return;

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
        else if (isMoving) heroState = HeroStateEnum.Move;
        else if (attack.IsAttacking) heroState = HeroStateEnum.Attack;
        else heroState = HeroStateEnum.Idle;
    }

    public void AttackStop()
    {
        heroState = HeroStateEnum.Idle;
        attack.StopIsAttacking();
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
}

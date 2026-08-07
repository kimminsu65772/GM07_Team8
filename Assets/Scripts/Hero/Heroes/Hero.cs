using TMPro;
using UnityEngine;

public abstract class Hero : MonoBehaviour
{
    [Header("영웅 정보")]
    [SerializeField] private string heroName;
    [SerializeField] private int heroLv;
    [SerializeField] private float heroMaxHP;
    [SerializeField] private float heroCurrentHP;
    [SerializeField] private float heroAtk;
    [SerializeField] private float heroDef;
    [SerializeField] private float heroCriChance;
    [SerializeField] private bool isDead;

    private float heroCriDamage = 2f;
    private HeroLocationEnum location;
    // private HeroAttackTypeEnum attackType;
    private int heroMaxLv = 3;
    protected IHeroStatTable statTable;

    [SerializeField] private TMP_Text name_T;

    [SerializeField] private float attackTime;
    private float attackTimer;

    [SerializeField] private Transform heroRoot;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private LayerMask enemyLayer;
    private GameObject targetEnemy;
    private bool isMoving;
    private bool canAttack;
    private bool isAttacking;
    private float searchRange = 50f;
    private float meleeRange;

    protected HeroStat stat;
    protected HeroStateEnum heroState;

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
        set => heroCurrentHP = Mathf.Max(0f, value);
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
    public float HeroAttackTime
    {
        get => attackTime;
        set => attackTime = Mathf.Max(0f, value);
    }
    public HeroStateEnum HeroState => heroState;

    protected virtual void Awake() { }

    protected virtual void Init(float attackTime,
        HeroLocationEnum location)
    {
        HeroLv = 1;
        LvApply(heroLv);
        heroCurrentHP = heroMaxHP;
        isDead = false;
        this.location = location;

        attackTimer = attackTime;
        isAttacking = false;
        meleeRange = transform.localScale.x / 2;

        if (name_T != null) name_T.text = heroName;

        heroState = HeroStateEnum.Idle;
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (targetEnemy == null) SearchEnemy();
        if (targetEnemy != null && location == HeroLocationEnum.Front) MoveToEnemy();
        if (targetEnemy != null && location == HeroLocationEnum.Back) Attack(targetEnemy);

        ChangeState();
    }

    public void TakeDamage(float amount)
    {
        // 방어력 적용하기
        heroCurrentHP -= amount;

        if (heroCurrentHP <= 0f)
        {
            Die();
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

        if (direction.sqrMagnitude <= meleeRange * meleeRange) canAttack = true;
        else canAttack = false;

        if (canAttack)
        {
            isMoving = false;
            Attack(targetEnemy);
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

    protected virtual void Attack(GameObject enemy)
    {
        float criRan = Random.Range(1f, 100f);
        float damage = heroAtk; // 적 방어력 적용

        if (attackTimer >= attackTime && !isAttacking)
        {
            isAttacking = true;
            attackTimer = 0f;

            if (criRan <= heroCriChance) damage *= heroCriDamage;

            // 공격 적용, 치명타 적용
            Debug.Log(gameObject.name + "의 공격, 피해량 : " + damage);
        }
    }

    public void AttackStop()
    {
        heroState = HeroStateEnum.Idle;
        isAttacking = false;
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    // 비용(매개변수) 지불 추가
    private void LvUp()
    {
        if (heroLv >= heroMaxLv) return;

        heroLv++;
        LvApply(heroLv);
    }

    private void LvApply(int lv)
    {
        stat = statTable.GetStat(lv);

        HeroMaxHP = stat.MaxHP;
        HeroAtk = stat.Atk;
        HeroDef = stat.Def;
    }

    private void ChangeState()
    {
        if (isDead) heroState = HeroStateEnum.Die;
        else if (isMoving) heroState = HeroStateEnum.Move;
        else if (isAttacking) heroState = HeroStateEnum.Attack;
        else heroState = HeroStateEnum.Idle;
    }

    private void OnDrawGizmos()
    {
        // 근거리 공격 사거리
        Gizmos.color = Color.red;
        if (location == HeroLocationEnum.Front) Gizmos.DrawWireSphere(
            new Vector3(transform.position.x, transform.position.y, transform.position.z), meleeRange);

        // 적 탐색 사거리
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(
            new Vector3(transform.position.x, transform.position.y, transform.position.z), searchRange);
    }
}

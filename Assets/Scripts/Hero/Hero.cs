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
    [SerializeField] private float heroCriticalChance;
    [SerializeField] private bool isDead;

    private HeroLocationEnum location;
    // private HeroAttackTypeEnum attackType;
    private int heroMaxLv = 3;
    protected IHeroStatTable statTable;

    [SerializeField] private TMP_Text name_T;

    [SerializeField] private float attackTime;
    private float attackTimer;

    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private LayerMask enemyLayer;
    private GameObject targetEnemy;
    private bool canAttack;
    private float searchRange = 100f;
    private float meleeRange = 1.5f;

    protected HeroStat stat;

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
    public float HeroDefence
    {
        get => heroDef;
        set => heroDef = Mathf.Max(0f, value);
    }
    public float HeroCriticalChance
    {
        get => heroCriticalChance;
        set => heroCriticalChance = Mathf.Clamp(value, 0f, 100f);
    }
    public bool IsDead => isDead;

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

        if (name_T != null) name_T.text = heroName;
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (targetEnemy == null) SearchEnemy();
        if (targetEnemy != null || location == HeroLocationEnum.Front) MoveToEnemy();
        if (targetEnemy != null || location == HeroLocationEnum.Back) Attack(targetEnemy);
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
        if (location == HeroLocationEnum.Back || targetEnemy == null) return;

        Vector2 direction = targetEnemy.transform.position - transform.position;

        if (direction.sqrMagnitude <= meleeRange * meleeRange) canAttack = true;
        else canAttack = false;

        if (canAttack)
        {
            Attack(targetEnemy);
        }
        else
        {
            transform.position += (Vector3)direction.normalized * moveSpeed * Time.deltaTime;
        }
    }

    protected virtual void Attack(GameObject enemy)
    {
        if (attackTimer >= attackTime)
        {
            attackTimer = 0f;
            // 공격
            Debug.Log(gameObject.name + "의 공격");
        }
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
        HeroDefence = stat.Def;
    }
}

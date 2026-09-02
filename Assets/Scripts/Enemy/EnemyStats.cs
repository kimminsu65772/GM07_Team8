using System.Collections;
using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [Header("Enemy Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Runtime Information")]
    [SerializeField] private int currentHealth;

    [Header("Stun")]
    [SerializeField] private bool canBeStunned = true;
    [SerializeField] private bool isStunned;

    private Coroutine stunCoroutine;
    [Header("Hit")]
    [SerializeField, Min(0f)]
    private float hitRadius = 1f;

   

    [Header("Death")]
    [SerializeField, Min(0f)]
    private float deathDestroyDelay = 1.2f;

    public event Action<EnemyStats> EnemyDamaged;
    public event Action<EnemyStats> EnemyDied;
    public event Action<EnemyStats> EnemyDeathCompleted;
    public event Action<EnemyStats> EnemyStunned;
    public event Action<EnemyStats> EnemyStunEnded;

    public int CurrentHealth => currentHealth;

    public int MaxHealth =>
        enemyData != null ? enemyData.MaxHealth : 0;

    public int AttackPower =>
        enemyData != null ? enemyData.AttackPower : 0;

    public float MoveSpeed =>
        enemyData != null ? enemyData.MoveSpeed : 0f;

    public float AttackRange =>
        enemyData != null ? enemyData.AttackRange : 0f;

    public float AttackInterval =>
        enemyData != null ? enemyData.AttackInterval : 1f;

    public bool IsBoss =>
    enemyData != null &&   enemyData.IsBoss;

    public bool IsDead => currentHealth <= 0;

    public bool IsStunned => isStunned;

    // IDamageable 인터페이스 구현
    public float HitRadius => hitRadius;

    private void Awake()
    {
        currentHealth = MaxHealth;
    }
    public void ResetForPool()
    {
        // 이전 사용에서 실행 중이던 사망 코루틴을 중단한다.
        StopAllCoroutines();
        stunCoroutine = null;
        isStunned = false;

        // 체력과 EnemyStats 상태를 초기화한다.
        currentHealth = MaxHealth;
        enabled = true;

        // 이전 이동 속도가 남지 않도록 정지시킨다.
        Rigidbody2D enemyRigidbody2D = GetComponent<Rigidbody2D>();

        if (enemyRigidbody2D != null)
        {
            enemyRigidbody2D.linearVelocity = Vector2.zero;
        }

        // 이동 컴포넌트를 다시 활성화한다.
        EnemyMovement enemyMovement = GetComponent<EnemyMovement>();

        if (enemyMovement != null)
        {
            enemyMovement.enabled = true;
        }

        // 일반 공격 컴포넌트를 다시 활성화한다.
        EnemyAttack enemyAttack = GetComponent<EnemyAttack>();

        if (enemyAttack != null)
        {
            enemyAttack.enabled = true;
        }

        // 사망 애니메이션과 이전 트리거를 초기화한다.
        EnemyAnimationController animationController =  GetComponent<EnemyAnimationController>();

        if (animationController != null)
        {
            animationController.ResetForPool();
        }
    }
    public void Stun(float duration)
    {
        if (!canBeStunned || IsBoss || IsDead || duration <= 0f)
        {
            return;
        }

        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        isStunned = true;

        Rigidbody2D enemyRigidbody2D =  GetComponent<Rigidbody2D>();

        if (enemyRigidbody2D != null)
        {
            enemyRigidbody2D.linearVelocity = Vector2.zero;
        }

        EnemyStunned?.Invoke(this);

        stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        isStunned = false;
        stunCoroutine = null;

        EnemyStunEnded?.Invoke(this);
    }
    // IDamageable 인터페이스 구현
    public void TakeDamage(DamageInfo damageInfo)
    {
        float damageAmount = (float)damageInfo.Damage;
        if (damageAmount <= 0f || IsDead)
        {
            return;
        }

        int finalDamage =
            Mathf.RoundToInt(damageAmount);

        currentHealth = Mathf.Max(
            currentHealth - finalDamage,
            0);
        //데미지 팝업
        if (DamageManager.Instance != null)
        {
            DamageManager.Instance.ShowDamage(damageInfo, transform.position);
        }

        EnemyDamaged?.Invoke(this);
        Debug.Log(
            $"{gameObject.name} 피격! 피해량: {finalDamage}, 남은 체력: {currentHealth}");

        if (IsDead)
        {
            Die();
            return;
        }
        Debug.Log($"EnemyStats TakeDamage 호출됨: {damageAmount}");
        
    }
    public void Heal(DamageInfo damageInfo){}

    private void Die()
    {
        EnemyDied?.Invoke(this);

        StartCoroutine(CompleteDeath());
    }

    private IEnumerator CompleteDeath()
    {
        yield return new WaitForSeconds(
            deathDestroyDelay);

        EnemyDeathCompleted?.Invoke(this);

      
    }
    
}
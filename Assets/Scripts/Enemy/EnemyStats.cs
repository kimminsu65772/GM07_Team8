using System.Collections;
using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [Header("Enemy Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Runtime Information")]
    [SerializeField] private int currentHealth;

    [Header("Hit")]
    [SerializeField, Min(0f)]
    private float hitRadius = 1f;

    /*[Header("Hit Stun")]
    // 피격 후 이동과 공격이 잠시 중단되는 시간
    [SerializeField, Min(0f)]
    private float hitStunDuration = 0.2f;

    private float hitStunEndTime;

    public bool IsHitStunned =>
        !IsDead && Time.time < hitStunEndTime;*/

    [Header("Death")]
    [SerializeField, Min(0f)]
    private float deathDestroyDelay = 1.2f;

    public event Action<EnemyStats> EnemyDamaged;
    public event Action<EnemyStats> EnemyDied;
    public event Action<EnemyStats> EnemyDeathCompleted;

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

    // IDamageable 인터페이스 구현
    public float HitRadius => hitRadius;

    private void Awake()
    {
        currentHealth = MaxHealth;
    }
    public void Stun()
    {
        // 추후 스턴 처리
    }
    // IDamageable 인터페이스 구현
    public void TakeDamage(DamageInfo damageInfo)
    {
        float damageAmount = damageInfo.Damage;
        if (damageAmount <= 0f || IsDead)
        {
            return;
        }

        int finalDamage =
            Mathf.RoundToInt(damageAmount);

        currentHealth = Mathf.Max(
            currentHealth - finalDamage,
            0);

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

        Destroy(gameObject);
    }
    
}
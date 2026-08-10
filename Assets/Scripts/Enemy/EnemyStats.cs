using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Runtime Information")]
    [SerializeField] private int currentHealth;

    [Header("Hit Stun")]
    // 피격 후 이동과 공격이 잠시 중단되는 시간
    [SerializeField, Min(0f)]
    private float hitStunDuration = 0.2f;

    private float hitStunEndTime;

    // 현재 피격 경직 상태인지 외부 스크립트에서 확인한다.
    public bool IsHitStunned =>
        !IsDead && Time.time < hitStunEndTime;

    [Header("Death")]
    // 사망 애니메이션이 재생될 시간을 확보한 뒤 오브젝트를 제거한다.
    [SerializeField, Min(0f)]
    private float deathDestroyDelay = 1.2f;

    // 적이 피해를 입었지만 아직 살아 있을 때 발생한다.
    public event Action<EnemyStats> EnemyDamaged;

    // 적의 체력이 0이 되어 사망했을 때 발생한다.
    public event Action<EnemyStats> EnemyDied;

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

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = MaxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        // 잘못된 피해량이거나 이미 죽은 적은 처리하지 않는다.
        if (damageAmount <= 0 || IsDead)
        {
            return;
        }

        currentHealth = Mathf.Max(
            currentHealth - damageAmount,
            0);

        if (IsDead)
        {
            // 치명타에서는 피격 모션 대신 바로 사망 모션을 재생한다.
            Die();
            return;
        }
        // 피해를 입을 때마다 경직 시간을 다시 시작한다.
        hitStunEndTime =
            Time.time + hitStunDuration;
        // 아직 살아 있다면 피격 애니메이션 실행을 알린다.
        EnemyDamaged?.Invoke(this);
    }

    private void Die()
    {
        // StageManager와 애니메이션 컨트롤러에 사망 사실을 알린다.
        EnemyDied?.Invoke(this);

        // 즉시 제거하지 않고 사망 애니메이션 재생 시간을 확보한다.
        Destroy(gameObject, deathDestroyDelay);
    }
}
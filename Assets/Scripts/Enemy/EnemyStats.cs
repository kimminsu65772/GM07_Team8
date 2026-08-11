using System.Collections;
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
    // 사망 모션이 끝나고 시체가 제거될 때 발생한다.
    // StageManager는 이 이벤트까지 기다린 후 다음 웨이브를 진행한다.
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
        // 논리적인 사망 처리는 HP가 0이 된 즉시 알린다.
        // 이동·공격 중단과 사망 애니메이션은 이 이벤트를 사용한다.
        EnemyDied?.Invoke(this);

        StartCoroutine(CompleteDeath());
    }

    private IEnumerator CompleteDeath()
    {
        // 사망 모션이 보이는 동안 오브젝트를 유지한다.
        yield return new WaitForSeconds(deathDestroyDelay);

        // 시체 제거가 완료될 시점을 StageManager에 알린다.
        EnemyDeathCompleted?.Invoke(this);

        Destroy(gameObject);
    }

   
}
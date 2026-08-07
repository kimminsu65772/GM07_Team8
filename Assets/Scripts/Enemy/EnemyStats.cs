using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private int currentHealth;

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
        if (damageAmount <= 0 || IsDead)
        {
            return;
        }

        currentHealth =
            Mathf.Max(currentHealth - damageAmount, 0);

        if (IsDead)
        {
            Die();
        }
    }

    private void Die()
    {
        EnemyDied?.Invoke(this);
        Destroy(gameObject);
    }
}
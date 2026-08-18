using UnityEngine;

[CreateAssetMenu(
    fileName = "NewEnemyData",
    menuName = "Game Data/Enemy Data"
)]
public class EnemyData : ScriptableObject
{
    [Header("Base Stats")]
    [SerializeField, Min(1)] private int maxHealth = 100;
    [SerializeField, Min(0)] private int attackPower = 10;
    [SerializeField, Min(0f)] private float moveSpeed = 2f;


    [Header("Attack")]
    [SerializeField, Min(0f)] private float attackRange = 2f;
    [SerializeField, Min(0.1f)] private float attackInterval = 1f;

    [Header("Type")]
    [SerializeField] private bool isBoss;


    public int MaxHealth => maxHealth;
    public int AttackPower => attackPower;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackInterval => attackInterval;

    public bool IsBoss => isBoss;
}
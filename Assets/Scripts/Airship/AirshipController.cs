using System;
using UnityEngine;

/// <summary>
/// 비행선의 총 컨트롤러.
/// </summary>
public class AirshipController : MonoBehaviour
{
    [SerializeField] private AirshipUpgradeController upgradeController;
    [SerializeField] private AirshipStatController statController;
    [SerializeField] private AirshipHealth health;
    [SerializeField] private AirshipAttack attack;
    [SerializeField] private AirshipMovement movement;
    [SerializeField] private AirshipEnemyChecker enemyChecker;
    
    private AirshipStateMachine stateMachine;

    public AirshipMovement Movement => movement;
    public AirshipEnemyChecker EnemyChecker => enemyChecker;
    private void Awake()
    {
        CacheComponents();
        BindEvents();
        stateMachine = new AirshipStateMachine(this);
        stateMachine.Init(stateMachine.IdleState);
    }

    private void OnDestroy()
    {
        UnbindEvents();
    }
    
    // 이걸 씬같은 곳에서 요청하기.
    public void Init()
    {
        upgradeController.Init();
        statController.Init(upgradeController.UpgradeState);
    }
    private void CacheComponents()
    {
        if (movement == null)
            movement = GetComponent<AirshipMovement>();

        if (enemyChecker == null)
            enemyChecker = GetComponent<AirshipEnemyChecker>();
        
        if (upgradeController == null)
            upgradeController = GetComponent<AirshipUpgradeController>();

        if (statController == null)
            statController = GetComponent<AirshipStatController>();

        if (health == null)
            health = GetComponent<AirshipHealth>();

        if (attack == null)
            attack = GetComponent<AirshipAttack>();
    }
    private void BindEvents()
    {
        upgradeController.OnUpgradeChanged += HandleUpgradeChanged;
        statController.OnStatsChanged += HandleStatsChanged;
        health.OnDestroyed += HandleDestroyed;
    }

    private void UnbindEvents()
    {
        upgradeController.OnUpgradeChanged -= HandleUpgradeChanged;
        statController.OnStatsChanged -= HandleStatsChanged;
        health.OnDestroyed -= HandleDestroyed;
    }
    private void HandleUpgradeChanged(AirshipUpgradeState upgradeState)
    {
        statController.Recalculate();
    }
    private void HandleStatsChanged(AirshipRuntimeStats stats)
    {
        health.ApplyStats(stats);
        movement.ApplyStats(stats);
        attack.ApplyStats(stats);
    }
    private void HandleDestroyed()
    {
        stateMachine.ChangeState(stateMachine.DestroyedState);
    }
    private void Update()
    {
        stateMachine.Tick();
    }
    public void Respawn()
    {
        health.ResetHealth();
        stateMachine.ChangeState(stateMachine.IdleState);
    }
    [ContextMenu("Test Damage")]
    private void TestDamage()
    {
        health.TakeDamage(150f);
    }

    [ContextMenu("Test Respawn")]
    private void TestRespawn()
    {
        Respawn();
    }
}

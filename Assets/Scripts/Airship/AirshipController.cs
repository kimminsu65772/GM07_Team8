using System;
using UnityEngine;

/// <summary>
/// 비행선의 총 컨트롤러.
/// </summary>
public class AirshipController : MonoBehaviour
{
    [SerializeField] private AirshipUpgradeController upgradeController;
    [SerializeField] private AirshipEquipmentController equipmentController;
    [SerializeField] private AirshipStatController statController;
    [SerializeField] private AirshipHealth health;
    [SerializeField] private AirshipAttack attack;
    [SerializeField] private AirshipMovement movement;
    [SerializeField] private AirshipEnemyChecker enemyChecker;
    
    private AirshipStateMachine stateMachine;

    public AirshipHealth Health => health;
    public AirshipUpgradeController UpgradeController => upgradeController;
    public AirshipEquipmentController EquipmentController => equipmentController;
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
        equipmentController.Init();
    }
    private void CacheComponents()
    {
        if (movement == null)
            movement = GetComponent<AirshipMovement>();

        if (enemyChecker == null)
            enemyChecker = GetComponent<AirshipEnemyChecker>();
        
        if (upgradeController == null)
            upgradeController = GetComponent<AirshipUpgradeController>();
        
        if (equipmentController == null)
            equipmentController =
                GetComponent<AirshipEquipmentController>();

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
        if (health.IsStunned)
        {
            movement.StopImmediately();
            return;
        }
        stateMachine.Tick();
    }
    public void Respawn()
    {
        statController.ResetTemporaryBuffs();
        health.ResetHealth();
        movement.StopImmediately();
        attack.ResetAttack();
        stateMachine.ChangeState(stateMachine.MoveForwardState);
    }
}

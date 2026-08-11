using System;
using UnityEngine;

/// <summary>
/// 비행선의 총 컨트롤러.
/// </summary>
public class AirshipController : MonoBehaviour
{
    [SerializeField] private AirshipUpgradeManager upgradeManager;
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

    private void Start()
    {
        // TODO 부트스트랩 생기고 데이터 관리가 생기면 이 테스트용 init은 삭제
        Init(new AirshipSaveData
        {
            AttackLevel = 3,
            DefenseLevel = 2,
            MaxHealthLevel = 5,
            CriticalLevel = 1
        });
    }

    private void OnDestroy()
    {
        UnbindEvents();
    }
    
    // 이걸 씬같은 곳에서 요청하기.
    // TODO 매니저 완전 삭제 후 데이터 인자 받는거 삭제
    public void Init(AirshipSaveData saveData)
    {
        upgradeManager.Init(saveData);
        statController.Init(upgradeManager.UpgradeState);
        upgradeController.Init();
        statController.Init(upgradeController.UpgradeState);
    }
    private void CacheComponents()
    {
        if (movement == null)
            movement = GetComponent<AirshipMovement>();

        if (enemyChecker == null)
            enemyChecker = GetComponent<AirshipEnemyChecker>();

        if (upgradeManager == null)
            upgradeManager = GetComponent<AirshipUpgradeManager>();
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
        upgradeManager.OnUpgradeChanged += HandleUpgradeChanged;
        upgradeController.OnUpgradeChanged += HandleUpgradeChanged;
        statController.OnStatsChanged += HandleStatsChanged;
        health.OnDestroyed += HandleDestroyed;
    }

    private void UnbindEvents()
    {
        upgradeManager.OnUpgradeChanged -= HandleUpgradeChanged;
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
        Debug.Log(stateMachine.CurrentState.StateType.ToString());
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

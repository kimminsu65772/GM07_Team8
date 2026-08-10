using UnityEngine;

public class AirshipAttack : MonoBehaviour
{
    [SerializeField] private AirshipEnemyChecker enemyChecker;
    [SerializeField] private AirshipHealth health;
    [SerializeField] private AirshipEquipmentController equipmentController;

    [Header("공격 포인트")]
    [SerializeField] private Transform aimPoint;
    [SerializeField] private Transform projectileSpawnPoint;

    private AirshipCannonData currentCannon;

    private float attackDamage;
    private float attackInterval = 1f;
    private float attackTimer;

    private Transform cachedTarget;
    private IDamageable cachedDamageable;

    private void Awake()
    {
        if (enemyChecker == null)
            enemyChecker = GetComponent<AirshipEnemyChecker>();

        if (health == null)
            health = GetComponent<AirshipHealth>();

        if (equipmentController == null)
        {
            equipmentController =
                GetComponent<AirshipEquipmentController>();
        }
    }

    private void OnEnable()
    {
        if (equipmentController == null)
            return;

        equipmentController.OnCannonChanged += HandleCannonChanged;

        // 초기 장착 대포 동기화
        HandleCannonChanged(equipmentController.EquippedCannon);
    }

    private void OnDisable()
    {
        if (equipmentController == null)
            return;

        equipmentController.OnCannonChanged -= HandleCannonChanged;
    }

    private void Update()
    {
        if (health != null && health.IsDestroyed)
            return;

        attackTimer -= Time.deltaTime;

        if (attackTimer > 0f)
            return;

        if (enemyChecker == null)
            return;

        Transform target = enemyChecker.FindNearestEnemy();

        if (target == null)
            return;

        Attack(target);
        attackTimer = attackInterval;
    }

    public void ApplyStats(AirshipRuntimeStats stats)
    {
        if (stats == null)
            return;

        attackDamage = stats.Attack;

        attackInterval =
            stats.AttackSpeed <= 0f
                ? 1f
                : 1f / stats.AttackSpeed;
    }

    private void HandleCannonChanged(AirshipCannonData cannon)
    {
        currentCannon = cannon;
    }

    private void Attack(Transform target)
    {
        if (currentCannon == null ||
            currentCannon.ProjectilePrefab == null ||
            aimPoint == null ||
            projectileSpawnPoint == null)
        {
            return;
        }

        if (target != cachedTarget)
        {
            cachedTarget = target;
            cachedDamageable =
                target.GetComponentInParent<IDamageable>();
        }

        if (cachedDamageable == null)
            return;

        RotateAimPoint(target);

        AirshipProjectileBase projectile = Instantiate(
            currentCannon.ProjectilePrefab,
            projectileSpawnPoint.position,
            aimPoint.rotation
        );

        projectile.Init(
            target,
            cachedDamageable,
            attackDamage
        );
    }

    private void RotateAimPoint(Transform target)
    {
        Vector2 direction =
            (Vector2)target.position -
            (Vector2)aimPoint.position;

        if (direction.sqrMagnitude <= 0f)
            return;

        aimPoint.right = direction.normalized;
    }
}
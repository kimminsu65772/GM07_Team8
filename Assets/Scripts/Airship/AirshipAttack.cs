using UnityEngine;

public class AirshipAttack : MonoBehaviour
{
    [SerializeField] private AirshipEnemyChecker enemyChecker;
    [SerializeField] private AirshipHealth health;
    [SerializeField] private AirshipEquipmentController equipmentController;

    [Header("공격 포인트")]
    [SerializeField] private Transform aimPoint;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("조준")]
    [SerializeField, Min(0f)] private float aimLerpSpeed = 25f;

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

        // 마지막으로 선택한 타겟을 계속 부드럽게 추적
        if (cachedTarget != null)
            RotateAimPoint(cachedTarget);

        if (attackTimer > 0f)
            return;

        if (enemyChecker == null)
            return;

        // 기존 타겟 선정 방식 유지
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

        // 발사 순간에도 한 번 회전하고 즉시 발사
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
        if (aimPoint == null || target == null)
            return;

        Vector2 direction =
            (Vector2)target.position -
            (Vector2)aimPoint.position;

        if (direction.sqrMagnitude <= 0f)
            return;

        float targetAngle =
            Mathf.Atan2(direction.y, direction.x) *
            Mathf.Rad2Deg;

        Quaternion targetRotation =
            Quaternion.Euler(0f, 0f, targetAngle);

        if (aimLerpSpeed <= 0f)
        {
            aimPoint.rotation = targetRotation;
            return;
        }

        aimPoint.rotation = Quaternion.Lerp(
            aimPoint.rotation,
            targetRotation,
            aimLerpSpeed * Time.deltaTime
        );
    }
}
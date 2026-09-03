using DG.Tweening;
using UnityEngine;

public class AirshipAttack : MonoBehaviour
{
    [SerializeField] private AirshipHeroChecker heroChecker;
    [SerializeField] private AirshipEnemyChecker enemyChecker;
    [SerializeField] private AirshipHealth health;
    [SerializeField] private AirshipEquipmentController equipmentController;

    [Header("공격 포인트")]
    [SerializeField] private Transform aimPoint;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("조준")]
    [SerializeField, Min(0f)] private float aimLerpSpeed = 25f;

    [Header("타겟 갱신")]
    [SerializeField, Min(0.01f)] private float targetRefreshInterval = 0.1f;
    
    [Header("타겟 없으면 복귀")]
    [SerializeField, Min(0f)] private float noTargetDelay = 0.7f;

    [SerializeField] private GameObject muzzleFlashVfx;
    private Tween muzzleFlashTween;

    private float noTargetTimer;

    private AirshipCannonData currentCannon;

    private double attackDamage;
    private float criticalChance;
    private float attackInterval = 1f;
    private float attackTimer;
    private float targetRefreshTimer;

    private Transform cachedTarget;
    private IDamageable cachedDamageable;

    private void Awake()
    {
        if (heroChecker == null) 
            heroChecker = GetComponent<AirshipHeroChecker>();
        if (enemyChecker == null)
            enemyChecker = GetComponent<AirshipEnemyChecker>();

        if (health == null)
            health = GetComponent<AirshipHealth>();

        if (equipmentController == null)
        {
            equipmentController =
                GetComponent<AirshipEquipmentController>();
        }
        
        muzzleFlashVfx.SetActive(false);
        muzzleFlashTween =
            DOVirtual.DelayedCall(
                    0.03f,
                    () => muzzleFlashVfx.SetActive(false)
                )
                .SetAutoKill(false)
                .Pause()
                .OnPlay(() =>
                {
                    muzzleFlashVfx.SetActive(false);
                    muzzleFlashVfx.SetActive(true);
                });
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
        if (health != null && health.IsStunned)
            return;
        
        if ((cachedDamageable is EnemyStats cachedEnemy &&
             cachedEnemy.IsDead) ||
            (cachedDamageable is Hero cachedHero &&
             (!cachedHero.isActiveAndEnabled ||
              cachedHero.IsDead)))
        {
            cachedTarget = null;
            cachedDamageable = null;
            targetRefreshTimer = 0f;
        }

        attackTimer -= Time.deltaTime;
        targetRefreshTimer -= Time.deltaTime;

        if (targetRefreshTimer <= 0f)
        {
            RefreshTarget();
            targetRefreshTimer = targetRefreshInterval;
        }

        // 마지막으로 선택한 타겟을 계속 부드럽게 추적
        if (cachedTarget != null)
        {
            noTargetTimer = 0f;

            RotateAimPoint(cachedTarget);
        }
        else
        {
            UpdateAimReturn();
        }

        if (attackTimer > 0f)
            return;

        if (cachedTarget == null)
            return;

        Attack(cachedTarget);
        attackTimer = attackInterval;
    }

    public void ResetAttack()
    {
        attackTimer = 0f;
        targetRefreshTimer = 0f;

        cachedTarget = null;
        cachedDamageable = null;
        
        noTargetTimer = 0f;

        aimPoint.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public void ApplyStats(AirshipRuntimeStats stats)
    {
        if (stats == null)
            return;

        attackDamage = stats.Attack;
        criticalChance = stats.CriticalChance;

        attackInterval =
            stats.AttackSpeed <= 0f
                ? 1f
                : 1f / stats.AttackSpeed;
    }

    private void HandleCannonChanged(AirshipCannonData cannon)
    {
        currentCannon = cannon;
        
        cachedTarget = null;
        cachedDamageable = null;
        targetRefreshTimer = 0f;
    }

    private void RefreshTarget()
    {
        if (currentCannon == null)
        {
            return;
        }

        Transform target;
        IDamageable damageable = null;

        if (currentCannon.CannonType == AirshipCannonType.Heal)
        {
            Hero hero = heroChecker?.FindHealTarget();

            target = hero?.transform;
            damageable = hero;
        }
        else
        {
            EnemyStats enemy =
                enemyChecker?.FindNearestEnemy();

            if (enemy == null)
            {
                target = null;
                damageable = null;
            }
            else
            {
                target =
                    enemy.TargetPoint != null
                        ? enemy.TargetPoint
                        : enemy.transform;

                damageable = enemy;
            }
        }

        // 같은 타겟이면 기존 캐시 유지
        if (target == cachedTarget)
        {
            return;
        }

        cachedTarget = target;

        // 회복탄과 일반탄 모두 감지 결과의 IDamageable을 직접 사용
        cachedDamageable = damageable;
    }

    private void Attack(Transform target)
    {
        PoolingManager poolingManager =
            PoolingManager.Instance;

        if (currentCannon == null ||
            currentCannon.ProjectilePrefab == null ||
            aimPoint == null ||
            projectileSpawnPoint == null ||
            poolingManager == null)
        {
            return;
        }

        if (target == null || cachedDamageable == null)
        {
            return;
        }

        if (!poolingManager.IsProjectilePrefabMatch(
                currentCannon.CannonType,
                currentCannon.ProjectilePrefab))
        {
            Debug.LogError(
                $"{currentCannon.CannonType}의 투사체 프리팹과 " +
                "PoolingManager의 프리팹이 다릅니다.",
                this
            );

            return;
        }

        RotateAimPoint(target);

        AirshipProjectileBase projectile =
            poolingManager.GetAirshipProjectile(
                currentCannon.CannonType
            );

        if (projectile == null)
        {
            return;
        }

        bool isHeal =
            currentCannon.CannonType == AirshipCannonType.Heal;

        bool isCritical =
            !isHeal &&
            (
                criticalChance >= 1f ||
                (criticalChance > 0f &&
                 Random.value < criticalChance)
            );

        double finalDamage =
            attackDamage * (isCritical ? 2d : 1d);

        if (currentCannon.FireSfxClip != null)
        {
            SoundManager.Instance.PlaySound(
                currentCannon.FireSfxClip,
                currentCannon.FireSfxVolume
            );
        }
        
        projectile.Init(
            projectileSpawnPoint.position,
            aimPoint.rotation,
            target,
            cachedDamageable,
            new DamageInfo(
                finalDamage,
                isCritical,
                isHeal
            )
        );
        PlayMuzzleFlash();
    }
    
    private void PlayMuzzleFlash()
    {
        if (muzzleFlashVfx == null ||
            muzzleFlashTween == null)
        {
            return;
        }
        muzzleFlashTween.Restart();
    }

    private void RotateAimPoint(Transform target)
    {
        if (aimPoint == null || target == null)
            return;
        
        Vector2 direction = (Vector2)target.position - (Vector2)aimPoint.position;

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
    private void UpdateAimReturn()
    {
        if (aimPoint == null)
            return;

        noTargetTimer += Time.deltaTime;

        if (noTargetTimer < noTargetDelay)
            return;

        if (aimLerpSpeed <= 0f)
        {
            aimPoint.localRotation = Quaternion.identity;
            return;
        }

        aimPoint.localRotation =
            Quaternion.Lerp(
                aimPoint.localRotation,
                Quaternion.identity,
                aimLerpSpeed * Time.deltaTime
            );
    }
}
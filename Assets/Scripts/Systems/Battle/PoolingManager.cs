using System;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance { get; private set; }

    [Header("Normal 투사체")]
    [SerializeField] private AirshipProjectileBase normalProjectilePrefab;
    [SerializeField, Min(0)] private int normalInitialSize = 10;

    [Header("Freeze 투사체")]
    [SerializeField] private AirshipProjectileBase freezeProjectilePrefab;
    [SerializeField, Min(0)] private int freezeInitialSize = 5;

    [Header("Rapid 투사체")]
    [SerializeField] private AirshipProjectileBase rapidProjectilePrefab;
    [SerializeField, Min(0)] private int rapidInitialSize = 10;

    [Header("Heal 투사체")]
    [SerializeField] private AirshipProjectileBase healProjectilePrefab;
    [SerializeField, Min(0)] private int healInitialSize = 5;

    [Header("Freeze 명중 VFX")]
    [SerializeField] private GameObject freezeImpactVfxPrefab;
    [SerializeField, Min(0)] private int freezeImpactVfxInitialSize = 3;

    private GenericObjectPool<AirshipProjectileBase> normalProjectilePool;
    private GenericObjectPool<AirshipProjectileBase> freezeProjectilePool;
    private GenericObjectPool<AirshipProjectileBase> rapidProjectilePool;
    private GenericObjectPool<AirshipProjectileBase> healProjectilePool;
    private GenericObjectPool<GameObject> freezeImpactVfxPool;

    [Space]
    [Header("영웅 투사체 1")]
    [SerializeField]
    private HeroAttackProjectileController heroProjectile1Prefab;

    [SerializeField, Min(0)]
    private int heroProjectile1InitialSize = 5;

    [Header("영웅 투사체 2")]
    [SerializeField]
    private HeroAttackProjectileController heroProjectile2Prefab;

    [SerializeField, Min(0)]
    private int heroProjectile2InitialSize = 5;

    [Header("영웅 투사체 3")]
    [SerializeField]
    private HeroAttackProjectileController heroProjectile3Prefab;

    [SerializeField, Min(0)]
    private int heroProjectile3InitialSize = 10;

    [Header("영웅 화살 투사체")]
    [SerializeField]
    private HeroAttackProjectileController heroArrowPrefab;

    [SerializeField, Min(0)]
    private int heroArrowInitialSize = 10;

    [Header("영웅 화살 스킬 투사체")]
    [SerializeField]
    private HeroAttackProjectileController heroArrowSkillPrefab;

    [SerializeField, Min(0)]
    private int heroArrowSkillInitialSize = 3;

    private GenericObjectPool<HeroAttackProjectileController> heroProjectile1Pool;
    private GenericObjectPool<HeroAttackProjectileController> heroProjectile2Pool;
    private GenericObjectPool<HeroAttackProjectileController> heroProjectile3Pool;
    private GenericObjectPool<HeroAttackProjectileController> heroArrowPool;
    private GenericObjectPool<HeroAttackProjectileController> heroArrowSkillPool;

    [Space]
    [Header("적 투사체")]
    [SerializeField]
    private EnemyProjectile enemyProjectilePrefab;

    [SerializeField, Min(0)]
    private int enemyProjectileInitialSize = 10;

    private GenericObjectPool<EnemyProjectile> enemyProjectilePool;

    [Space]
    [Header("데미지 팝업")]
    [SerializeField] private DamagePopup damagePopupPrefab;
    [SerializeField, Min(0)] private int damagePopupInitialSize = 20;

    private GenericObjectPool<DamagePopup> damagePopupPool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // 중복 매니저 제거용
            Destroy(gameObject);
            return;
        }

        Instance = this;

        normalProjectilePool = CreateComponentPool(
            normalProjectilePrefab,
            normalInitialSize,
            "Normal 투사체 프리팹이 지정되지 않았습니다.",
            () => CreateAirshipProjectile(
                normalProjectilePrefab,
                AirshipCannonType.Normal));

        freezeProjectilePool = CreateComponentPool(
            freezeProjectilePrefab,
            freezeInitialSize,
            "Freeze 투사체 프리팹이 지정되지 않았습니다.",
            () => CreateAirshipProjectile(
                freezeProjectilePrefab,
                AirshipCannonType.Freeze));

        rapidProjectilePool = CreateComponentPool(
            rapidProjectilePrefab,
            rapidInitialSize,
            "Rapid 투사체 프리팹이 지정되지 않았습니다.",
            () => CreateAirshipProjectile(
                rapidProjectilePrefab,
                AirshipCannonType.Rapid));

        healProjectilePool = CreateComponentPool(
            healProjectilePrefab,
            healInitialSize,
            "Heal 투사체 프리팹이 지정되지 않았습니다.",
            () => CreateAirshipProjectile(
                healProjectilePrefab,
                AirshipCannonType.Heal));

        freezeImpactVfxPool = CreateGameObjectPool(
            freezeImpactVfxPrefab,
            freezeImpactVfxInitialSize,
            "Freeze 명중 VFX 프리팹이 지정되지 않았습니다.",
            CreateFreezeImpactVfx);

        heroProjectile1Pool = CreateComponentPool(
            heroProjectile1Prefab,
            heroProjectile1InitialSize,
            "PlayerAttackProjectile1 영웅 투사체 프리팹이 없습니다.",
            () => CreateHeroProjectile(
                heroProjectile1Prefab,
                HeroProjectileType.PlayerAttackProjectile1));

        heroProjectile2Pool = CreateComponentPool(
            heroProjectile2Prefab,
            heroProjectile2InitialSize,
            "PlayerAttackProjectile2 영웅 투사체 프리팹이 없습니다.",
            () => CreateHeroProjectile(
                heroProjectile2Prefab,
                HeroProjectileType.PlayerAttackProjectile2));

        heroProjectile3Pool = CreateComponentPool(
            heroProjectile3Prefab,
            heroProjectile3InitialSize,
            "PlayerAttackProjectile3 영웅 투사체 프리팹이 없습니다.",
            () => CreateHeroProjectile(
                heroProjectile3Prefab,
                HeroProjectileType.PlayerAttackProjectile3));

        heroArrowPool = CreateComponentPool(
            heroArrowPrefab,
            heroArrowInitialSize,
            "PlayerAttackArrow 영웅 투사체 프리팹이 없습니다.",
            () => CreateHeroProjectile(
                heroArrowPrefab,
                HeroProjectileType.PlayerAttackArrow));

        heroArrowSkillPool = CreateComponentPool(
            heroArrowSkillPrefab,
            heroArrowSkillInitialSize,
            "PlayerSkillArrow 영웅 투사체 프리팹이 없습니다.",
            () => CreateHeroProjectile(
                heroArrowSkillPrefab,
                HeroProjectileType.PlayerSkillArrow));

        enemyProjectilePool = CreateComponentPool(
            enemyProjectilePrefab,
            enemyProjectileInitialSize,
            "적 투사체 프리팹이 지정되지 않았습니다.",
            CreateEnemyProjectile);

        damagePopupPool = CreateComponentPool(
            damagePopupPrefab,
            damagePopupInitialSize,
            "데미지 팝업 프리팹이 지정되지 않았습니다.",
            CreateDamagePopup);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private GenericObjectPool<T> CreateComponentPool<T>(
        T prefab,
        int initialSize,
        string missingPrefabMessage,
        Func<T> createFunc)
        where T : Component
    {
        if (prefab == null)
        {
            Debug.LogError(missingPrefabMessage, this);
            return null;
        }

        return CreatePool(
            createFunc,
            initialSize,
            item => item.gameObject.SetActive(false),
            item => Destroy(item.gameObject));
    }

    private GenericObjectPool<GameObject> CreateGameObjectPool(
        GameObject prefab,
        int initialSize,
        string missingPrefabMessage,
        Func<GameObject> createFunc)
    {
        if (prefab == null)
        {
            Debug.LogError(missingPrefabMessage, this);
            return null;
        }

        return CreatePool(
            createFunc,
            initialSize,
            item => item.SetActive(false),
            item => Destroy(item));
    }

    private GenericObjectPool<T> CreatePool<T>(
        Func<T> createFunc,
        int initialSize,
        Action<T> onRelease,
        Action<T> onDestroy)
        where T : class
    {
        GenericObjectPool<T> pool = new GenericObjectPool<T>(
            createFunc,
            null,
            onRelease,
            onDestroy,
            true,
            initialSize,
            int.MaxValue);

        pool.Prewarm(initialSize);

        return pool;
    }

    private AirshipProjectileBase CreateAirshipProjectile(
        AirshipProjectileBase prefab,
        AirshipCannonType projectileType)
    {
        AirshipProjectileBase projectile =
            Instantiate(prefab, transform);

        projectile.SetPoolingManager(
            this,
            projectileType);

        projectile.gameObject.SetActive(false);
        return projectile;
    }

    public AirshipProjectileBase GetAirshipProjectile(
        AirshipCannonType projectileType)
    {
        GenericObjectPool<AirshipProjectileBase> pool =
            GetAirshipProjectilePool(projectileType);

        AirshipProjectileBase prefab =
            GetProjectilePrefab(projectileType);

        if (pool == null || prefab == null)
        {
            Debug.LogError(
                $"{projectileType} 투사체 풀 설정이 잘못되었습니다.",
                this);
            return null;
        }

        return pool.Get();
    }

    public void ReleaseAirshipProjectile(
        AirshipProjectileBase projectile,
        AirshipCannonType projectileType)
    {
        if (projectile == null ||
            !projectile.gameObject.activeSelf)
        {
            return;
        }

        GenericObjectPool<AirshipProjectileBase> pool =
            GetAirshipProjectilePool(projectileType);

        if (pool == null)
        {
            Debug.LogError(
                $"{projectileType} 투사체 풀을 찾을 수 없습니다.",
                this);

            projectile.gameObject.SetActive(false);
            return;
        }

        pool.Release(projectile);
    }

    private GenericObjectPool<AirshipProjectileBase>
        GetAirshipProjectilePool(AirshipCannonType projectileType)
    {
        switch (projectileType)
        {
            case AirshipCannonType.Normal:
                return normalProjectilePool;

            case AirshipCannonType.Freeze:
                return freezeProjectilePool;

            case AirshipCannonType.Rapid:
                return rapidProjectilePool;

            case AirshipCannonType.Heal:
                return healProjectilePool;

            default:
                return null;
        }
    }

    private AirshipProjectileBase GetProjectilePrefab(
        AirshipCannonType projectileType)
    {
        switch (projectileType)
        {
            case AirshipCannonType.Normal:
                return normalProjectilePrefab;

            case AirshipCannonType.Freeze:
                return freezeProjectilePrefab;

            case AirshipCannonType.Rapid:
                return rapidProjectilePrefab;

            case AirshipCannonType.Heal:
                return healProjectilePrefab;

            default:
                return null;
        }
    }

    public bool IsProjectilePrefabMatch(
        AirshipCannonType projectileType,
        AirshipProjectileBase expectedPrefab)
    {
        return expectedPrefab != null &&
               GetProjectilePrefab(projectileType) == expectedPrefab;
    }

    private GameObject CreateFreezeImpactVfx()
    {
        GameObject vfx = Instantiate(
            freezeImpactVfxPrefab,
            transform);

        VfxAnimationEventReceiver[] receivers =
            vfx.GetComponentsInChildren<VfxAnimationEventReceiver>(true);

        foreach (VfxAnimationEventReceiver receiver in receivers)
        {
            receiver.SetPoolingManager(this, vfx);
        }

        vfx.SetActive(false);
        return vfx;
    }

    public GameObject GetFreezeImpactVfx(
        Vector3 position,
        Quaternion rotation)
    {
        if (freezeImpactVfxPool == null)
        {
            return null;
        }

        GameObject vfx = freezeImpactVfxPool.Get();

        vfx.transform.SetPositionAndRotation(position, rotation);
        vfx.transform.localScale = Vector3.one;
        vfx.SetActive(true);

        Animator[] animators =
            vfx.GetComponentsInChildren<Animator>(true);

        foreach (Animator animator in animators)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        return vfx;
    }

    public void ReleaseFreezeImpactVfx(GameObject vfx)
    {
        if (vfx == null || !vfx.activeSelf)
        {
            return;
        }

        if (freezeImpactVfxPool == null)
        {
            vfx.SetActive(false);
            return;
        }

        freezeImpactVfxPool.Release(vfx);
    }

    private HeroAttackProjectileController CreateHeroProjectile(
        HeroAttackProjectileController prefab,
        HeroProjectileType projectileType)
    {
        HeroAttackProjectileController projectile =
            Instantiate(prefab, transform);

        projectile.SetPoolingManager(this, projectileType);
        projectile.gameObject.SetActive(false);
        return projectile;
    }

    public HeroAttackProjectileController GetHeroProjectile(
        HeroProjectileType projectileType)
    {
        GenericObjectPool<HeroAttackProjectileController> pool =
            GetHeroProjectilePool(projectileType);

        HeroAttackProjectileController prefab =
            GetHeroProjectilePrefab(projectileType);

        if (pool == null || prefab == null)
        {
            Debug.LogError(
                $"{projectileType} 영웅 투사체 풀이 없습니다.",
                this);
            return null;
        }

        return pool.Get();
    }

    public void ReleaseHeroProjectile(
        HeroAttackProjectileController projectile,
        HeroProjectileType projectileType)
    {
        if (projectile == null ||
            !projectile.gameObject.activeSelf)
        {
            return;
        }

        GenericObjectPool<HeroAttackProjectileController> pool =
            GetHeroProjectilePool(projectileType);

        if (pool == null)
        {
            Debug.LogError(
                $"{projectileType} 영웅 투사체 반환 풀이 없습니다.",
                this);

            projectile.gameObject.SetActive(false);
            return;
        }

        pool.Release(projectile);
    }

    private GenericObjectPool<HeroAttackProjectileController>
        GetHeroProjectilePool(HeroProjectileType projectileType)
    {
        switch (projectileType)
        {
            case HeroProjectileType.PlayerAttackProjectile1:
                return heroProjectile1Pool;

            case HeroProjectileType.PlayerAttackProjectile2:
                return heroProjectile2Pool;

            case HeroProjectileType.PlayerAttackProjectile3:
                return heroProjectile3Pool;

            case HeroProjectileType.PlayerAttackArrow:
                return heroArrowPool;

            case HeroProjectileType.PlayerSkillArrow:
                return heroArrowSkillPool;

            default:
                return null;
        }
    }

    private HeroAttackProjectileController GetHeroProjectilePrefab(
        HeroProjectileType projectileType)
    {
        switch (projectileType)
        {
            case HeroProjectileType.PlayerAttackProjectile1:
                return heroProjectile1Prefab;

            case HeroProjectileType.PlayerAttackProjectile2:
                return heroProjectile2Prefab;

            case HeroProjectileType.PlayerAttackProjectile3:
                return heroProjectile3Prefab;

            case HeroProjectileType.PlayerAttackArrow:
                return heroArrowPrefab;

            case HeroProjectileType.PlayerSkillArrow:
                return heroArrowSkillPrefab;

            default:
                return null;
        }
    }

    private EnemyProjectile CreateEnemyProjectile()
    {
        EnemyProjectile projectile =
            Instantiate(enemyProjectilePrefab, transform);

        projectile.SetPoolingManager(this);
        projectile.gameObject.SetActive(false);
        return projectile;
    }

    public EnemyProjectile GetEnemyProjectile()
    {
        if (enemyProjectilePool == null)
        {
            return null;
        }

        return enemyProjectilePool.Get();
    }

    public void ReleaseEnemyProjectile(EnemyProjectile projectile)
    {
        if (projectile == null ||
            !projectile.gameObject.activeSelf)
        {
            return;
        }

        if (enemyProjectilePool == null)
        {
            projectile.gameObject.SetActive(false);
            return;
        }

        enemyProjectilePool.Release(projectile);
    }

    private DamagePopup CreateDamagePopup()
    {
        DamagePopup popup =
            Instantiate(damagePopupPrefab, transform);

        popup.SetPoolingManager(this);
        popup.gameObject.SetActive(false);
        return popup;
    }

    public DamagePopup GetDamagePopup(
        Vector3 position,
        Transform parent)
    {
        if (damagePopupPool == null || parent == null)
        {
            return null;
        }

        DamagePopup popup = damagePopupPool.Get();

        popup.transform.SetParent(parent, false);
        popup.transform.SetPositionAndRotation(
            position,
            Quaternion.identity);

        popup.gameObject.SetActive(true);
        return popup;
    }

    public void ReleaseDamagePopup(DamagePopup popup)
    {
        if (popup == null ||
            !popup.gameObject.activeSelf)
        {
            return;
        }

        popup.transform.SetParent(transform, false);

        if (damagePopupPool == null)
        {
            popup.gameObject.SetActive(false);
            return;
        }

        damagePopupPool.Release(popup);
    }
}

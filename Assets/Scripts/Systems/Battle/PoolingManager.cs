using System.Collections.Generic;
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

    private readonly List<AirshipProjectileBase> inactiveNormalProjectiles =
        new List<AirshipProjectileBase>();

    private readonly List<AirshipProjectileBase> inactiveFreezeProjectiles =
        new List<AirshipProjectileBase>();

    private readonly List<AirshipProjectileBase> inactiveRapidProjectiles =
        new List<AirshipProjectileBase>();

    private readonly List<GameObject> inactiveFreezeImpactVfx =
        new List<GameObject>();
    private readonly List<AirshipProjectileBase> inactiveHealProjectiles =
        new List<AirshipProjectileBase>();
    
    
    
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

    private readonly List<HeroAttackProjectileController>
        inactiveHeroProjectile1 =
            new List<HeroAttackProjectileController>();

    private readonly List<HeroAttackProjectileController>
        inactiveHeroProjectile2 =
            new List<HeroAttackProjectileController>();

    private readonly List<HeroAttackProjectileController>
        inactiveHeroProjectile3 =
            new List<HeroAttackProjectileController>();

    private readonly List<HeroAttackProjectileController>
        inactiveHeroArrow =
            new List<HeroAttackProjectileController>();

    private readonly List<HeroAttackProjectileController>
        inactiveHeroArrowSkill =
            new List<HeroAttackProjectileController>();




    [Space]
    [Header("적 투사체")]
    [SerializeField]
    private EnemyProjectile enemyProjectilePrefab;

    [SerializeField, Min(0)]
    private int enemyProjectileInitialSize = 10;

    private readonly List<EnemyProjectile>
        inactiveEnemyProjectiles =
            new List<EnemyProjectile>();
    
    [Space]
    [Header("데미지 팝업")]
    [SerializeField] private DamagePopup damagePopupPrefab;
    [SerializeField, Min(0)] private int damagePopupInitialSize = 20;

    private readonly List<DamagePopup> inactiveDamagePopups =
        new List<DamagePopup>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // 중복 매니저 제거용
            Destroy(gameObject);
            return;
        }

        Instance = this;

        PrewarmProjectile(
            normalProjectilePrefab,
            normalInitialSize,
            inactiveNormalProjectiles,
            AirshipCannonType.Normal
        );

        PrewarmProjectile(
            freezeProjectilePrefab,
            freezeInitialSize,
            inactiveFreezeProjectiles,
            AirshipCannonType.Freeze
        );

        PrewarmProjectile(
            rapidProjectilePrefab,
            rapidInitialSize,
            inactiveRapidProjectiles,
            AirshipCannonType.Rapid
        );
        PrewarmProjectile(
            healProjectilePrefab,
            healInitialSize,
            inactiveHealProjectiles,
            AirshipCannonType.Heal
        );

        PrewarmFreezeImpactVfx();
        
        
        
        PrewarmHeroProjectile(
            heroProjectile1Prefab,
            heroProjectile1InitialSize,
            inactiveHeroProjectile1,
            HeroProjectileType.PlayerAttackProjectile1
        );

        PrewarmHeroProjectile(
            heroProjectile2Prefab,
            heroProjectile2InitialSize,
            inactiveHeroProjectile2,
            HeroProjectileType.PlayerAttackProjectile2
        );

        PrewarmHeroProjectile(
            heroProjectile3Prefab,
            heroProjectile3InitialSize,
            inactiveHeroProjectile3,
            HeroProjectileType.PlayerAttackProjectile3
        );

        PrewarmHeroProjectile(
            heroArrowPrefab,
            heroArrowInitialSize,
            inactiveHeroArrow,
            HeroProjectileType.PlayerAttackArrow
        );

        PrewarmHeroProjectile(
            heroArrowSkillPrefab,
            heroArrowSkillInitialSize,
            inactiveHeroArrowSkill,
            HeroProjectileType.PlayerSkillArrow
        );


        PrewarmEnemyProjectile();
        
        PrewarmDamagePopup();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void PrewarmProjectile(
        AirshipProjectileBase prefab,
        int count,
        List<AirshipProjectileBase> pool,
        AirshipCannonType projectileType)
    {
        if (prefab == null)
        {
            Debug.LogError(
                $"{projectileType} 투사체 프리팹이 지정되지 않았습니다.",
                this
            );
            return;
        }

        for (int i = 0; i < count; i++)
        {
            pool.Add(
                CreateProjectile(
                    prefab,
                    projectileType
                )
            );
        }
    }

    private AirshipProjectileBase CreateProjectile(
        AirshipProjectileBase prefab,
        AirshipCannonType projectileType)
    {
        AirshipProjectileBase projectile =
            Instantiate(prefab, transform);

        projectile.SetPoolingManager(
            this,
            projectileType
        );

        projectile.gameObject.SetActive(false);

        return projectile;
    }

    public AirshipProjectileBase GetAirshipProjectile(
        AirshipCannonType projectileType)
    {
        List<AirshipProjectileBase> pool =
            GetProjectilePool(projectileType);

        AirshipProjectileBase prefab =
            GetProjectilePrefab(projectileType);

        if (pool == null || prefab == null)
        {
            Debug.LogError(
                $"{projectileType} 투사체 풀 설정이 잘못되었습니다.",
                this
            );
            return null;
        }

        AirshipProjectileBase projectile;

        if (pool.Count > 0)
        {
            int lastIndex = pool.Count - 1;

            projectile = pool[lastIndex];
            pool.RemoveAt(lastIndex);
        }
        else
        {
            projectile =
                CreateProjectile(
                    prefab,
                    projectileType
                );
        }

        // 위치 설정과 활성화는 Init에서 처리
        return projectile;
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

        List<AirshipProjectileBase> pool =
            GetProjectilePool(projectileType);

        if (pool == null)
        {
            Debug.LogError(
                $"{projectileType} 투사체 풀을 찾을 수 없습니다.",
                this
            );

            projectile.gameObject.SetActive(false);
            return;
        }

        projectile.gameObject.SetActive(false);
        pool.Add(projectile);
    }

    private List<AirshipProjectileBase> GetProjectilePool(
        AirshipCannonType projectileType)
    {
        switch (projectileType)
        {
            case AirshipCannonType.Normal:
                return inactiveNormalProjectiles;

            case AirshipCannonType.Freeze:
                return inactiveFreezeProjectiles;

            case AirshipCannonType.Rapid:
                return inactiveRapidProjectiles;
            
            case AirshipCannonType.Heal:
                return inactiveHealProjectiles;

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

    private void PrewarmFreezeImpactVfx()
    {
        if (freezeImpactVfxPrefab == null)
        {
            Debug.LogError(
                "Freeze 명중 VFX 프리팹이 지정되지 않았습니다.",
                this
            );
            return;
        }

        for (int i = 0; i < freezeImpactVfxInitialSize; i++)
        {
            inactiveFreezeImpactVfx.Add(
                CreateFreezeImpactVfx()
            );
        }
    }

    private GameObject CreateFreezeImpactVfx()
    {
        GameObject vfx =
            Instantiate(
                freezeImpactVfxPrefab,
                transform
            );

        VfxAnimationEventReceiver[] receivers =
            vfx.GetComponentsInChildren<
                VfxAnimationEventReceiver
            >(true);

        foreach (
            VfxAnimationEventReceiver receiver
            in receivers)
        {
            receiver.SetPoolingManager(
                this,
                vfx
            );
        }

        vfx.SetActive(false);

        return vfx;
    }

    public GameObject GetFreezeImpactVfx(
        Vector3 position,
        Quaternion rotation)
    {
        if (freezeImpactVfxPrefab == null)
        {
            return null;
        }

        GameObject vfx;

        if (inactiveFreezeImpactVfx.Count > 0)
        {
            int lastIndex =
                inactiveFreezeImpactVfx.Count - 1;

            vfx = inactiveFreezeImpactVfx[lastIndex];
            inactiveFreezeImpactVfx.RemoveAt(lastIndex);
        }
        else
        {
            vfx = CreateFreezeImpactVfx();
        }

        vfx.transform.SetPositionAndRotation(
            position,
            rotation
        );

        vfx.transform.localScale = Vector3.one;
        vfx.gameObject.SetActive(true);

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

        vfx.SetActive(false);
        inactiveFreezeImpactVfx.Add(vfx);
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    private void PrewarmHeroProjectile(
    HeroAttackProjectileController prefab,
    int count,
    List<HeroAttackProjectileController> pool,
    HeroProjectileType projectileType)
    {
        if (prefab == null)
        {
            Debug.LogError(
                $"{projectileType} 영웅 투사체 프리팹이 없습니다.",
                this
            );
            return;
        }

        for (int i = 0; i < count; i++)
        {
            pool.Add(
                CreateHeroProjectile(
                    prefab,
                    projectileType
                )
            );
        }
    }

    private HeroAttackProjectileController CreateHeroProjectile(
        HeroAttackProjectileController prefab,
        HeroProjectileType projectileType)
    {
        HeroAttackProjectileController projectile =
            Instantiate(prefab, transform);

        projectile.SetPoolingManager(
            this,
            projectileType
        );

        projectile.gameObject.SetActive(false);

        return projectile;
    }

    public HeroAttackProjectileController GetHeroProjectile(
        HeroProjectileType projectileType)
    {
        List<HeroAttackProjectileController> pool =
            GetHeroProjectilePool(projectileType);

        HeroAttackProjectileController prefab =
            GetHeroProjectilePrefab(projectileType);

        if (pool == null || prefab == null)
        {
            Debug.LogError(
                $"{projectileType} 영웅 투사체 풀이 없습니다.",
                this
            );
            return null;
        }

        HeroAttackProjectileController projectile;

        if (pool.Count > 0)
        {
            int lastIndex = pool.Count - 1;

            projectile = pool[lastIndex];
            pool.RemoveAt(lastIndex);
        }
        else
        {
            projectile =
                CreateHeroProjectile(
                    prefab,
                    projectileType
                );
        }

        return projectile;
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

        List<HeroAttackProjectileController> pool =
            GetHeroProjectilePool(projectileType);

        if (pool == null)
        {
            Debug.LogError(
                $"{projectileType} 영웅 투사체 반환 풀이 없습니다.",
                this
            );

            projectile.gameObject.SetActive(false);
            return;
        }

        projectile.gameObject.SetActive(false);
        pool.Add(projectile);
    }

    private List<HeroAttackProjectileController>
        GetHeroProjectilePool(
            HeroProjectileType projectileType)
    {
        switch (projectileType)
        {
            case HeroProjectileType.PlayerAttackProjectile1:
                return inactiveHeroProjectile1;

            case HeroProjectileType.PlayerAttackProjectile2:
                return inactiveHeroProjectile2;

            case HeroProjectileType.PlayerAttackProjectile3:
                return inactiveHeroProjectile3;

            case HeroProjectileType.PlayerAttackArrow:
                return inactiveHeroArrow;

            case HeroProjectileType.PlayerSkillArrow:
                return inactiveHeroArrowSkill;

            default:
                return null;
        }
    }

    private HeroAttackProjectileController
        GetHeroProjectilePrefab(
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
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    private void PrewarmEnemyProjectile()
    {
        if (enemyProjectilePrefab == null)
        {
            Debug.LogError(
                "적 투사체 프리팹이 지정되지 않았습니다.",
                this
            );
            return;
        }

        for (int i = 0; i < enemyProjectileInitialSize; i++)
        {
            inactiveEnemyProjectiles.Add(
                CreateEnemyProjectile()
            );
        }
    }

    private EnemyProjectile CreateEnemyProjectile()
    {
        EnemyProjectile projectile =
            Instantiate(
                enemyProjectilePrefab,
                transform
            );

        projectile.SetPoolingManager(this);
        projectile.gameObject.SetActive(false);

        return projectile;
    }

    public EnemyProjectile GetEnemyProjectile()
    {
        EnemyProjectile projectile;

        if (inactiveEnemyProjectiles.Count > 0)
        {
            int lastIndex =
                inactiveEnemyProjectiles.Count - 1;

            projectile =
                inactiveEnemyProjectiles[lastIndex];

            inactiveEnemyProjectiles.RemoveAt(lastIndex);
        }
        else
        {
            projectile = CreateEnemyProjectile();
        }

        return projectile;
    }

    public void ReleaseEnemyProjectile(
        EnemyProjectile projectile)
    {
        if (projectile == null ||
            !projectile.gameObject.activeSelf)
        {
            return;
        }

        projectile.gameObject.SetActive(false);
        inactiveEnemyProjectiles.Add(projectile);
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    private void PrewarmDamagePopup()
    {
        if (damagePopupPrefab == null)
        {
            Debug.LogError(
                "데미지 팝업 프리팹이 지정되지 않았습니다.",
                this
            );
            return;
        }

        for (int i = 0; i < damagePopupInitialSize; i++)
        {
            inactiveDamagePopups.Add(
                CreateDamagePopup()
            );
        }
    }

    private DamagePopup CreateDamagePopup()
    {
        DamagePopup popup =
            Instantiate(
                damagePopupPrefab,
                transform
            );

        popup.SetPoolingManager(this);
        popup.gameObject.SetActive(false);

        return popup;
    }

    public DamagePopup GetDamagePopup(
        Vector3 position,
        Transform parent)
    {
        if (damagePopupPrefab == null ||
            parent == null)
        {
            return null;
        }

        DamagePopup popup;

        if (inactiveDamagePopups.Count > 0)
        {
            int lastIndex =
                inactiveDamagePopups.Count - 1;

            popup =
                inactiveDamagePopups[lastIndex];

            inactiveDamagePopups.RemoveAt(lastIndex);
        }
        else
        {
            popup = CreateDamagePopup();
        }

        popup.transform.SetParent(parent, false);
        popup.transform.SetPositionAndRotation(
            position,
            Quaternion.identity
        );

        popup.gameObject.SetActive(true);

        return popup;
    }

    public void ReleaseDamagePopup(
        DamagePopup popup)
    {
        if (popup == null ||
            !popup.gameObject.activeSelf)
        {
            return;
        }

        popup.gameObject.SetActive(false);
        popup.transform.SetParent(transform, false);

        inactiveDamagePopups.Add(popup);
    }
}
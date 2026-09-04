using System.Collections;
using UnityEngine;

public enum HeroProjectileType
{
    None,
    PlayerAttackProjectile1,
    PlayerAttackProjectile2,
    PlayerAttackProjectile3,
    PlayerAttackArrow,
    PlayerSkillArrow
}

public class HeroAttackProjectileController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField, Min(0.1f)] private float lifeTime = 3f;

    [SerializeField] protected AudioSource projectileAudio;

    protected Hero hero;
    protected Transform targetPos;
    protected EnemyStats target;

    protected DamageInfo damageInfo;
    public double damageBonus { get; private set; }

    private float remainingLifeTime;

    // 마지막으로 추적하던 위치
    protected Vector3 targetPosition;

    // 타겟이 사라졌거나 죽었는지
    protected bool isTargetLost;

    [Header("Attack Effect Preset")]
    [SerializeField] protected Vector2 posPreset = new Vector2(0f, 0f);
    [SerializeField] protected Vector2 scalePreset = new Vector2(4f, 4f);
    [SerializeField] protected Vector3 rotationPreset = new Vector3(0f, 0f, 0f);

    [SerializeField] protected EffectPlayer vfx;
    protected Coroutine effectCoroutine;

    private PoolingManager poolingManager;
    private HeroProjectileType poolingType;

    public void SetPoolingManager(
        PoolingManager poolingManager,
        HeroProjectileType poolingType)
    {
        this.poolingManager = poolingManager;
        this.poolingType = poolingType;
    }

    public void Init(Hero hero, Vector3 startPosition, Quaternion startRotation, Transform target, double damageBonus)
    {
        transform.SetPositionAndRotation(startPosition, startRotation);

        this.hero = hero;
        targetPos = target;
        this.target = targetPos != null ? targetPos.GetComponentInParent<EnemyStats>() : null;

        // 처음 타겟 위치 저장
        if (targetPos != null)
        {
            targetPosition = targetPos.position;
        }
        
        isTargetLost = false;

        remainingLifeTime = lifeTime;

        this.damageBonus = damageBonus;

        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        if (vfx != null) effectCoroutine = StartCoroutine(PlayEffectLoop());
    }

    protected void OnDisable()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
            effectCoroutine = null;
        }
    }

    protected virtual void Update()
    {
        remainingLifeTime -= Time.deltaTime;

        if (remainingLifeTime <= 0f)
        {
            ReturnToPool();
            return;
        }

        UpdateTargetPosition();
        MoveToTargetPosition();
        CheckHit();
    }

    protected virtual void UpdateTargetPosition()
    {
        // 아직 타겟을 추적 중인 경우
        if (!isTargetLost)
        {
            if (targetPos == null)
            {
                isTargetLost = true;
                return;
            }

            if (target != null && target.IsDead)
            {
                // 죽기 직전 마지막 위치 저장
                targetPosition = targetPos.position;

                isTargetLost = true;
                targetPos = null;

                return;
            }

            // 살아있는 타겟은 계속 위치 갱신
            targetPosition = targetPos.position;
        }
    }

    protected virtual void MoveToTargetPosition()
    {
        Vector2 direction = targetPosition - transform.position;

        SetRotation(direction);

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // 타겟이 사라진 상태에서
        // 마지막 위치까지 도착했으면 종료
        if (isTargetLost && Vector2.Distance(transform.position, targetPosition) <= 0.05f) OnReachTargetPosition();
    }

    protected virtual void CheckHit()
    {
        if (isTargetLost) return;
        if (target == null || target.IsDead) return;
        if (!target.TryGetComponent<IDamageable>(out IDamageable enemy)) return;

        float distance = Vector2.Distance(transform.position, targetPos.position);

        if (distance <= enemy.HitRadius) OnHitTarget(enemy);
    }

    protected virtual void OnHitTarget(IDamageable enemy)
    {
        damageInfo = hero.Attack.GetDamageInfo(damageBonus);
        enemy.TakeDamage(damageInfo);

        if (projectileAudio != null) SoundManager.Instance.PlaySound(projectileAudio.clip, 2f);

        ReturnToPool();
    }

    protected virtual void OnReachTargetPosition()
    {
        ReturnToPool();
    }

    protected void SetRotation(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.001f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    protected void ReturnToPool()
    {
        if (poolingManager == null)
        {
            gameObject.SetActive(false);
            return;
        }

        poolingManager.ReleaseHeroProjectile(this, poolingType);
    }

    protected IEnumerator PlayEffectLoop()
    {
        while (true)
        {
            if (vfx != null)
            {
                vfx.PlayAttackEffect(posPreset, scalePreset, rotationPreset);
            }

            yield return new WaitForSeconds(vfx.FrameTime * vfx.AttackFrames.Length);
        }
    }
}
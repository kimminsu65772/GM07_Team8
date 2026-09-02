using System.Collections;
using UnityEngine;

public class ArcherSkillProjectile : HeroAttackProjectileController
{
    [SerializeField] private EffectPlayer vfx;

    private Vector2 posPreset = new Vector2(0f, 0.8f);
    private Vector2 scalePreset = new Vector2(5f, 5f);

    private Hero hero;
    private Transform target;

    [SerializeField] private float skillMoveSpeed = 15f;

    private Vector3 targetPosition;
    private bool isTargetDead = false;
    private bool isExploded;

    private Coroutine effectCoroutine;

    private void OnEnable()
    {
        isTargetDead = false;
        isExploded = false;

        StartCoroutine(PlayEffectLoop());
    }

    private void OnDisable()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
            effectCoroutine = null;
        }
    }

    public void Init(Hero hero, GameObject target)
    {
        this.hero = hero;
        this.target = target != null ? target.transform : null;

        isTargetDead = false;
        isExploded = false;

        if (this.target != null)
        {
            targetPosition = this.target.position;
            SetRotationToTarget();
        }

        gameObject.SetActive(true);
    }

    protected override void Update()
    {
        if (isExploded)
            return;

        // 타겟이 살아있는 상태
        if (!isTargetDead)
        {
            if (target == null)
            {
                Explode();
                return;
            }

            if (target.TryGetComponent<EnemyStats>(
                    out EnemyStats enemyStats))
            {
                if (enemyStats.IsDead)
                {
                    // 죽은 순간의 위치를 저장
                    targetPosition = target.position;

                    isTargetDead = true;
                    target = null;

                    SetRotationToTargetPosition();

                    return;
                }
            }

            targetPosition = target.position;

            SetRotationToTarget();

            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                skillMoveSpeed * Time.deltaTime
            );

            HitEnemy();

            return;
        }

        // 타겟이 죽은 상태
        // 죽은 적의 마지막 위치까지 이동
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            skillMoveSpeed * Time.deltaTime
        );

        SetRotationToTargetPosition();

        if (Vector2.Distance(
                transform.position,
                targetPosition) <= 0.05f)
        {
            Explode();
        }
    }

    private IEnumerator PlayEffectLoop()
    {
        while (true)
        {
            if (vfx != null)
            {
                vfx.PlayAttackEffect(
                    Vector2.zero,
                    new Vector2(0.7f, 0.7f),
                    transform.eulerAngles +
                    new Vector3(0f, 0f, 180f)
                );
            }

            yield return new WaitForSeconds(0.05f * 6);
        }
    }

    private void HitEnemy()
    {
        if (target == null)
            return;

        if (!target.TryGetComponent<IDamageable>(
                out IDamageable enemy))
        {
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            target.position
        );

        if (distance <= enemy.HitRadius)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (isExploded)
            return;

        isExploded = true;

        transform.position = targetPosition;

        if (hero != null)
        {
            if (hero.Attack != null)
            {
                hero.Attack.AreaAttack(
                    0,
                    transform,
                    4f,
                    1.8f
                );
            }

            if (hero.Attack != null &&
                hero.Attack.VFX != null)
            {
                hero.Attack.VFX.PlayTargetEffect(
                    transform,
                    posPreset,
                    scalePreset
                );
            }
        }

        ReturnToPool();
    }

    private void SetRotationToTarget()
    {
        if (target == null)
            return;

        Vector2 direction =
            target.position - transform.position;

        SetRotation(direction);
    }

    private void SetRotationToTargetPosition()
    {
        Vector2 direction =
            targetPosition - transform.position;

        SetRotation(direction);
    }

    private void SetRotation(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.001f)
            return;

        float angle =
            Mathf.Atan2(direction.y, direction.x) *
            Mathf.Rad2Deg - 90f;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }
}
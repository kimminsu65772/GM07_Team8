using System.Collections;
using UnityEngine;

public class ArcherSkillProjectile : MonoBehaviour
{
    [SerializeField] private EffectPlayer vfx;

    private Vector2 posPreset = new Vector2(0f, 0.5f);
    private Vector2 scalePreset = new Vector2(5f, 5f);

    private Hero hero;
    private Transform target;

    [SerializeField] private float moveSpeed = 15f;

    private Vector3 targetPosition;
    private bool isTargetDead = false;

    private void Start()
    {
        StartCoroutine(PlayEffectLoop());
        Destroy(gameObject, 5f);
    }

    public void Init(Hero hero, GameObject target)
    {
        this.hero = hero;
        this.target = target.transform;

        targetPosition = target.transform.position;

        SetRotationToTarget();
    }

    private void Update()
    {
        if (!isTargetDead)
        {
            if (target == null)
            {
                Explode();
                return;
            }

            if (target.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
            {
                if (enemyStats.IsDead)
                {
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
                moveSpeed * Time.deltaTime
            );

            HitEnemy();

            return;
        }

        transform.position =
            Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime
        );

        SetRotationToTargetPosition();

        if (Vector2.Distance(transform.position, targetPosition) <= 0.05f) Explode();
    }

    private IEnumerator PlayEffectLoop()
    {
        while (true)
        {
            vfx.PlayAttackEffect(
                Vector2.zero, new Vector2(0.7f, 0.7f), transform.eulerAngles + new Vector3(0f, 0f, 180f));

            yield return new WaitForSeconds(0.05f * 6);
        }
    }

    private void HitEnemy()
    {
        if (target == null) return;

        if (!target.TryGetComponent<IDamageable>(out IDamageable enemy)) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= enemy.HitRadius) Explode();
    }

    private void Explode()
    {
        transform.position = targetPosition;

        hero.Attack.AreaAttack(0, transform, 4f, 1.8f);
        hero.Attack.VFX.PlayTargetEffect(transform, posPreset, scalePreset);

        Destroy(gameObject);
    }

    private void SetRotationToTarget()
    {
        if (target == null) return;

        Vector2 direction = target.position - transform.position;

        SetRotation(direction);
    }

    private void SetRotationToTargetPosition()
    {
        Vector2 direction = targetPosition - transform.position;

        SetRotation(direction);
    }

    private void SetRotation(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.001f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
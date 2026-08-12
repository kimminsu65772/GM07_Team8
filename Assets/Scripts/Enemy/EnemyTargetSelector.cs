using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyAttack))]
public class EnemyTargetSelector : MonoBehaviour
{
    [Header("Fallback Target")]
    [SerializeField] private Transform airshipTarget;

    [Header("Hero Search")]
    [SerializeField] private LayerMask heroLayerMask;

    private EnemyMovement enemyMovement;
    private EnemyAttack enemyAttack;
    private Transform currentTarget;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    private void Start()
    {
        RefreshTarget();
    }

    private void Update()
    {
        Transform priorityTarget =
             GetPriorityTarget(transform.position);

        if (priorityTarget == currentTarget)
        {
            return;
        }

        currentTarget = priorityTarget;

        // 영웅 또는 비행선으로 공격 대상만 변경
        enemyAttack?.SetTarget(currentTarget);
        enemyMovement?.SetCombatTarget(currentTarget);
    }

    public void SetAirshipTarget(Transform newAirshipTarget)
    {
        airshipTarget = newAirshipTarget;

        // 이동은 항상 비행선 방향
        enemyMovement?.SetTarget(airshipTarget);

        // 공격 타깃은 영웅 우선으로 다시 탐색
        RefreshTarget();
    }

    public void RefreshTarget()
    {
        currentTarget =
            GetPriorityTarget(transform.position);

        enemyAttack?.SetTarget(currentTarget);
        enemyMovement?.SetCombatTarget(currentTarget);
    }

    public Transform GetPriorityTarget(Vector3 enemyPosition)
    {
        Transform closestHero = null;
        float closestDistance = float.MaxValue;

        Transform[] sceneTransforms =
            FindObjectsByType<Transform>(
                FindObjectsSortMode.None);

        foreach (Transform candidate in sceneTransforms)
        {
            if (!candidate.gameObject.activeInHierarchy)
            {
                continue;
            }

            int candidateLayerMask =
                1 << candidate.gameObject.layer;

            // Hero Layer가 아니면 제외
            if ((heroLayerMask.value & candidateLayerMask) == 0)
            {
                continue;
            }
            // 죽은 영웅은 타깃에서 제외
            Hero hero = candidate.GetComponentInParent<Hero>();
            if (hero == null ||
                 hero.IsDead ||
                 hero.HeroCurrentHP <= 0f)
            {
                continue;
            }
            // 적보다 뒤쪽에 있는 영웅은 제외
            if (candidate.position.x > enemyPosition.x)
            {
                continue;
            }

            float distance = Vector2.Distance(
                enemyPosition,
                candidate.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHero = candidate;
            }
        }

        return closestHero != null
            ? closestHero
            : airshipTarget;
    }
}
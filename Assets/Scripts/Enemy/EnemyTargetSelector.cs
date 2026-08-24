using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]

public class EnemyTargetSelector : MonoBehaviour
{
    [Header("Fallback Target")]
    [SerializeField] private Transform airshipTarget;

    [Header("Hero Search")]
    [SerializeField] private LayerMask heroLayerMask;

    private EnemyMovement enemyMovement;
    private EnemyAttack enemyAttack;
    private Transform currentTarget;
    private EnemyRangedAttack enemyRangedAttack;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyRangedAttack = GetComponent<EnemyRangedAttack>();
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
        enemyRangedAttack?.SetTarget(currentTarget);
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
        enemyRangedAttack?.SetTarget(currentTarget);
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

            Hero hero =
                candidate.GetComponentInParent<Hero>();

            if (hero == null)
            {
                continue;
            }



            // 죽어 있는 영웅은 제외
            if (hero.IsDead ||
                hero.HeroCurrentHP <= 0f)
            {
                Debug.Log(  $"{name}: 죽은 영웅 제외 | " + $"{hero.name}, IsDead: {hero.IsDead}, HP: {hero.HeroCurrentHP}" );
                continue;
            }

            float distance =
                Vector2.Distance(
                    enemyPosition,
                    candidate.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHero = candidate;
            }
        }

        // 살아있는 영웅이 하나라도 있으면 무조건 영웅 우선
        if (closestHero != null)
        {
            return closestHero;
        }

        // 살아있는 영웅이 없을 때만 비행선
        return airshipTarget;
    }
}
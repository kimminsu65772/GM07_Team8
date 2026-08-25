using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(EnemyStats))]
public class EnemyMagicAttack : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float explosionEffectDuration = 1f;
    [SerializeField] private float castDelay = 0.3f;
    [SerializeField] private float explosionYOffset = 0.5f;
    [SerializeField] private float explosionEffectScale = 0.02f;

    private Transform target;
    private IDamageable targetDamageable;
    private EnemyStats enemyStats;
    private Coroutine castCoroutine;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
    }

    private void OnDisable()
    {
        if (castCoroutine != null)
        {
            StopCoroutine(castCoroutine);
            castCoroutine = null;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetDamageable = target != null ? target.GetComponentInParent<IDamageable>() : null;
    }

    public void BeginCast()
    {
        if (!isActiveAndEnabled || enemyStats == null || enemyStats.IsDead)
        {
            return;
        }

        if (castCoroutine != null)
        {
            StopCoroutine(castCoroutine);
        }

        castCoroutine = StartCoroutine(CastAfterDelay());
    }

    private IEnumerator CastAfterDelay()
    {
        yield return new WaitForSeconds(castDelay);
        castCoroutine = null;
        CastExplosion();
    }

    public void CastExplosion()
    {
        if (target == null || targetDamageable == null || enemyStats == null || enemyStats.IsDead)
        {
            return;
        }

        Vector3 explosionPosition = target.position + Vector3.up * explosionYOffset;

        if (explosionEffectPrefab != null)
        {
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, explosionPosition, Quaternion.identity);
            Canvas explosionCanvas = explosionEffect.GetComponent<Canvas>();
            RectTransform explosionRect = explosionEffect.GetComponent<RectTransform>();

            if (explosionCanvas != null)
            {
                explosionCanvas.renderMode = RenderMode.WorldSpace;
                explosionCanvas.overrideSorting = true;
                SortingGroup targetSortingGroup = target.GetComponentInParent<SortingGroup>();
                if (targetSortingGroup != null)
                {
                    explosionCanvas.sortingLayerID = targetSortingGroup.sortingLayerID;
                    explosionCanvas.sortingOrder = targetSortingGroup.sortingOrder + 10;
                }
                else
                {
                    explosionCanvas.sortingLayerName = "Default";
                    explosionCanvas.sortingOrder = 100;
                }
            }

            if (explosionRect != null)
            {
                explosionRect.position = explosionPosition;
                explosionRect.sizeDelta = new Vector2(64f, 64f);
                explosionRect.localScale = Vector3.one * explosionEffectScale;
            }

            Destroy(explosionEffect, explosionEffectDuration);
        }

        targetDamageable.TakeDamage(new DamageInfo(enemyStats.AttackPower));
    }
}
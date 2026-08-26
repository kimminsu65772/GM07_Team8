using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotDamageArea : MonoBehaviour
{
    [Header("Area")]
    [SerializeField, Min(0.1f)] private float radius = 2.5f;
    [SerializeField] private LayerMask damageableLayerMask;

    [Header("Damage")]
    [SerializeField, Min(0.1f)] private float duration = 5f;
    [SerializeField, Min(0.1f)] private float damageInterval = 1f;
    [SerializeField, Min(0f)] private float damageMultiplier = 0.3f;

    private float bossAttackPower;

    public void Initialize(float attackPower)
    {
        bossAttackPower = attackPower;
        StartCoroutine(DamageRoutine());
    }

    private IEnumerator DamageRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            ApplyDamage();

            yield return new WaitForSeconds(damageInterval);
            elapsedTime += damageInterval;
        }

        Destroy(gameObject);
    }

    private void ApplyDamage()
    {
        // 현재 장판 범위 안에 있는 영웅을 모두 찾는다.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius, damageableLayerMask);

        // 한 영웅에게 Collider가 여러 개 있어도 한 번만 피해를 주도록 기록한다.
        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();

        foreach (Collider2D hitCollider in hitColliders)
        {
            IDamageable damageable = hitCollider.GetComponentInParent<IDamageable>();

            if (damageable == null || damagedTargets.Contains(damageable))
            {
                continue;
            }

            damagedTargets.Add(damageable);

            float dotDamage = bossAttackPower * damageMultiplier;
            damageable.TakeDamage(new DamageInfo(dotDamage));
        }
    }

    // 선택한 장판의 실제 피해 범위를 Scene 창에서 표시한다.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
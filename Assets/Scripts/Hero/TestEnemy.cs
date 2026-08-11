using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable
{
    public float HitRadius { get; } = 1f;
    public void TakeDamage(float damage)
    {
        Debug.Log($"{gameObject.name} 피격, {damage} 피해");
    }
}

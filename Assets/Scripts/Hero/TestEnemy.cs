using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable
{
    public float HitRadius { get; } = 1f;
    public void TakeDamage(DamageInfo damageInfo)
    {
        // Debug.Log($"{gameObject.name} 피격, {damageInfo.Damage} 피해");
    }
    public void Stun(float duration){}
    public void Heal(DamageInfo damageInfo){}
}

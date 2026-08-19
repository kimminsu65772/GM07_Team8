public readonly struct DamageInfo
{
    public readonly float Damage;
    public readonly bool IsCritical;

    public DamageInfo(float damage, bool isCritical = false)
    {
        Damage = damage;
        IsCritical = isCritical;
    }
}
public interface IDamageable
{
    float HitRadius { get; }
    void TakeDamage(DamageInfo damageInfo);
    void Stun(float duration);
}
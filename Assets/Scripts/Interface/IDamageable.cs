public readonly struct DamageInfo
{
    public readonly float Damage;
    public readonly bool IsCritical;
    public readonly bool IsHeal;

    public DamageInfo(float damage, bool isCritical = false, bool isHeal = false)
    {
        Damage = damage;
        IsCritical = isCritical;
        IsHeal = isHeal;
    }
}
public interface IDamageable
{
    float HitRadius { get; }
    void TakeDamage(DamageInfo damageInfo);
    void Stun(float duration);
    void Heal(DamageInfo damageInfo);
}
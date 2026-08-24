public readonly struct DamageInfo
{
    public readonly double Damage;
    public readonly bool IsCritical;
    public readonly bool IsHeal;

    public DamageInfo(double damage, bool isCritical = false, bool isHeal = false)
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
public interface IDamageable
{
    float HitRadius { get; }
    void TakeDamage(float damage);
}
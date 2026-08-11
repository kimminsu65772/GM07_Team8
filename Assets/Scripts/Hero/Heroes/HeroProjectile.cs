using UnityEngine;

public class HeroProjectile : MonoBehaviour
{
    private Hero hero;
    private Transform target;

    public void Init(Hero hero, Transform target)
    {
        this.hero = hero;
        this.target = target;
    }

    private void ThrowToEnemy()
    {
        
    }
}

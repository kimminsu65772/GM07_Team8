using UnityEngine;

public class EnemyAnimationEventReceiver : MonoBehaviour
{
    private EnemyAttack enemyAttack;

    private void Awake()
    {
        enemyAttack =
            GetComponentInParent<EnemyAttack>();
    }

    public void ApplyAttackDamage()
    {
        if (enemyAttack == null)
        {
            return;
        }

        enemyAttack.ApplyAttackDamage();
    }
}
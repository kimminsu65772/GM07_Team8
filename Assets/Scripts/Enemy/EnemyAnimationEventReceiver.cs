using UnityEngine;

public class EnemyAnimationEventReceiver : MonoBehaviour
{
    private EnemyAttack enemyAttack;
    private BossChargeSkill bossChargeSkill;

    private void Awake()
    {
        enemyAttack =
            GetComponentInParent<EnemyAttack>();

        bossChargeSkill =
            GetComponentInParent<BossChargeSkill>();
    }

    public void ApplyAttackDamage()
    {
        if (enemyAttack == null)
        {
            return;
        }

        enemyAttack.ApplyAttackDamage();
    }
    public void FireProjectile()
    {
       
        EnemyRangedAttack rangedAttack =
            GetComponentInParent<EnemyRangedAttack>();

        if (rangedAttack != null)
        {
            rangedAttack.FireProjectile();
        }
    }
    public void StartCharge()
    {
        if (bossChargeSkill == null)
        {
            return;
        }

        bossChargeSkill.StartCharge();
    }

    public void AttackStop()
    {
    }
}
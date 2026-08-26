using UnityEngine;

public class EnemyAnimationEventReceiver : MonoBehaviour
{
    private EnemyAttack enemyAttack;
    private BossChargeSkill bossChargeSkill;
    private EnemyMagicAttack enemyMagicAttack;
    private BossDotAreaSkill bossDotAreaSkill;

    private void Awake()
    {
        enemyAttack = GetComponentInParent<EnemyAttack>();

        enemyMagicAttack = GetComponentInParent<EnemyMagicAttack>();

        bossChargeSkill =  GetComponentInParent<BossChargeSkill>();

        bossDotAreaSkill = GetComponentInParent<BossDotAreaSkill>();
    }

    public void ApplyAttackDamage()
    {
        // 마법 공격 컴포넌트가 있으면 폭발 공격 실행
        if (enemyMagicAttack != null)
        {
            enemyMagicAttack.CastExplosion();
            return;
        }

        // 일반 근거리 공격
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
    public void CastDotArea()
    {
        if (bossDotAreaSkill == null)
        {
            return;
        }

        bossDotAreaSkill.CastDotArea();
    }
    public void AttackStop()
    {
    }
}
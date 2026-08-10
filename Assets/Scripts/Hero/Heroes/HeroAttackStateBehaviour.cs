using UnityEngine;

public class HeroAttackStateBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        Hero hero = animator.GetComponentInParent<Hero>();

        if (hero != null)
        {
            hero.AttackStop();
        }
    }
}

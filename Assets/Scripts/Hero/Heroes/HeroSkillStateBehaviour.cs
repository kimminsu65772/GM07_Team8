using UnityEngine;

public class HeroSkillStateBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        Hero hero = animator.GetComponentInParent<Hero>();

        if (hero != null)
        {
            hero.SkillStop();
        }
    }
}

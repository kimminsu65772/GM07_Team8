using UnityEngine;

public class HeroActionStateBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        Hero hero = animator.GetComponentInParent<Hero>();

        if (hero == null)
            return;

        switch (hero.HeroState)
        {
            case HeroStateEnum.Attack:
                hero.AttackStop();
                break;

            case HeroStateEnum.Skill:
                hero.SkillStop();
                break;
        }
    }
}
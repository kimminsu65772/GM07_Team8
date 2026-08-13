using UnityEngine;

public class HeroActionStateBehaviour : StateMachineBehaviour
{
    public enum ActionType
    {
        Attack,
        Skill
    }

    [SerializeField] private ActionType actionType;

    public override void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        Hero hero = animator.GetComponentInParent<Hero>();

        if (hero == null)
            return;

        switch (actionType)
        {
            case ActionType.Attack:
                hero.AttackStop();
                break;

            case ActionType.Skill:
                hero.SkillStop();
                break;
        }
    }
}
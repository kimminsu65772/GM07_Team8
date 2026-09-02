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
                Debug.Log(hero.name + "공격 초기화");
                break;

            case HeroStateEnum.Skill:
                hero.SkillStop();
                Debug.Log(hero.name + "스킬 초기화");
                break;
        }
    }
}
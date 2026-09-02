using UnityEngine;

public class HeroAnimationController : MonoBehaviour
{
    [SerializeField] Hero hero;
    [SerializeField] SPUM_Prefabs ani;
    private Animator animator;

    private HeroStateEnum currentState;

    void Awake()
    {
        currentState = hero.HeroState;
        ani.OverrideControllerInit();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        AniApply(hero.HeroState);
    }

    private void AniApply(HeroStateEnum state)
    {
        if (currentState == state) return;
        
        currentState = state;

        switch (state)
        {
            case HeroStateEnum.Idle:
                ani.PlayAnimation(PlayerState.IDLE, 0);
                break;

            case HeroStateEnum.Move:
                ani.PlayAnimation(PlayerState.MOVE, 0);
                break;

            case HeroStateEnum.Attack:
                ani.PlayAnimation(PlayerState.ATTACK, 0);
                break;

            case HeroStateEnum.Skill:
                ani.PlayAnimation(PlayerState.ATTACK, 1);
                break;

            case HeroStateEnum.Stunned:
                ani.PlayAnimation(PlayerState.DEBUFF, 0);
                break;

            case HeroStateEnum.Die:
                ani.PlayAnimation(PlayerState.DEATH, 0);
                break;
        }
    }

    public void ResetPose()
    {
        animator.Play("IDLE", 0, 0f);
        animator.Update(0f);
    }
}

using UnityEngine;

public class AirshipDestroyedState : AirshipStateBase
{
    public override AirshipStateType StateType => AirshipStateType.Destroyed;

    public AirshipDestroyedState(AirshipController controller, AirshipStateMachine stateMachine)
        : base(controller, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Controller.Movement.StopImmediately();
    }
}
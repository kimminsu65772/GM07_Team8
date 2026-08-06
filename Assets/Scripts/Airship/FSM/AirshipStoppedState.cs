using UnityEngine;

public sealed class AirshipStoppedState : AirshipStateBase
{
    public override AirshipStateType StateType => AirshipStateType.Stopped;
    public AirshipStoppedState(AirshipController controller, AirshipStateMachine stateMachine)
        : base(controller, stateMachine)
    {
    }

    public override void Tick()
    {
        base.Tick();
        // 범위 안에 적이 있는지 매프레임 체크
        if (!Controller.EnemyChecker.HasEnemy())
        {
            // 없으면 전진상태로 전환
            StateMachine.ChangeState(StateMachine.MoveForwardState);
            return;
        }
        // 즉시 멈추는 범위 안에 적이 있는지 매프레임 체크
        if (Controller.EnemyChecker.HasImmediateStopEnemy())
        {
            // 있으면 바로 멈춤
            Controller.Movement.StopImmediately();
            return;
        }
        // 있으면 감속 하면서 멈춤
        Controller.Movement.Stop();
    }
}

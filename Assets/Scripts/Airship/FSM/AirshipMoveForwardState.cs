using UnityEngine;

public class AirshipMoveForwardState : AirshipStateBase
{
    public override AirshipStateType StateType => AirshipStateType.MoveForward;
    public AirshipMoveForwardState(AirshipController controller, AirshipStateMachine stateMachine)
        : base(controller, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Tick()
    {
        base.Tick();
        if (Input.GetKey(KeyCode.W))
        {
            StateMachine.ChangeState(StateMachine.IdleState);
        }
        // 범위 안에 적이 있는지 매프레임 체크
        if (Controller.EnemyChecker.HasEnemy())
        {
            // 있으면 멈춤 상태 진입
            StateMachine.ChangeState(StateMachine.StoppedState);
            return;
        }
        // 없으면 계속 움직이기
        Controller.Movement.MoveForward();
    }
}

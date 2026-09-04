using UnityEngine;

// 일단 만들어 놓긴 했는데 쓸데가 딱히생각안난다. 스테이지나 던전 진입 후 모든 준비가 끝나기 전까진 이상태로 둬야하나? 
public class AirshipIdleState : AirshipStateBase
{
    public override AirshipStateType StateType => AirshipStateType.Idle;
    public AirshipIdleState(AirshipController controller, AirshipStateMachine stateMachine)
        : base(controller, stateMachine)
    {
    }

    public override void Tick()
    {
        base.Tick();
    }
}

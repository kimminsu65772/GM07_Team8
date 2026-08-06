using UnityEngine;

public enum AirshipStateType
{
    Idle,
    MoveForward,
    Stopped,
    Destroyed,
}
public class AirshipStateMachine
{
    public AirshipStateBase CurrentState { get; private set; }
    public AirshipIdleState IdleState { get; private set; }
    public AirshipMoveForwardState MoveForwardState { get; private set; }
    public AirshipStoppedState StoppedState { get; private set; }
    public AirshipDestroyedState DestroyedState { get; private set; }
    
    public AirshipStateMachine(AirshipController controller)
    {
        IdleState = new AirshipIdleState(controller, this);
        MoveForwardState = new AirshipMoveForwardState(controller, this);
        StoppedState = new AirshipStoppedState(controller, this);
        DestroyedState = new AirshipDestroyedState(controller, this);
    }

    public void Init(AirshipStateBase startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
    }
    public void ChangeState(AirshipStateBase nextState)
    {
        if (CurrentState == nextState)
        {
            return;
        }

        CurrentState?.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }
    public void Tick()
    {
        CurrentState?.Tick();
    }
}

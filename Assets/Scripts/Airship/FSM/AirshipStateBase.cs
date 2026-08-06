using UnityEngine;

public abstract class AirshipStateBase
{
    protected readonly AirshipController Controller;
    protected readonly AirshipStateMachine StateMachine;
    public abstract AirshipStateType StateType { get; }

    protected AirshipStateBase(AirshipController controller, AirshipStateMachine stateMachine)
    {
        this.Controller = controller;
        this.StateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Tick() { }
    public virtual void Exit() { }
}

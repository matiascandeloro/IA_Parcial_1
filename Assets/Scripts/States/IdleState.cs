using UnityEngine;

public class IdleState : State
{
    float _timeToChangePatrol = 3f;
    float _timer = 0f;
    public IdleState(StateMachine stateMachine) : base(stateMachine) { 
    }
    public override void Enter() 
    {
        _timer = 0f;
        Debug.Log("Entering Idle State");
    }
    public override void Exit() 
    {
        Debug.Log("Exiting Idle State");
    }
    public override void Update() 
    { 

        _timer += Time.deltaTime;

        if (_timer > _timeToChangePatrol) {
            Debug.Log("Me muevo");
            _stateMachine.ChangeState(PoliceStates.Patrol);
        }
        Debug.Log("Updating Idle State"); 
    }
}

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
    }
    public override void Exit() 
    {
    }
    public override void Update() 
    { 

        _timer += Time.deltaTime;

        if (_timer > _timeToChangePatrol) {
            _stateMachine.ChangeState(HunterStates.Patrol);
        }
    }
}

using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum PoliceStates
{
    Idle,
    Patrol
}


public class FSMAgent : MonoBehaviour
{
    [SerializeField] private PatrolData patrolData;
    public float speed = 3f;
    private StateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = new StateMachine();

        IdleState idleState = new IdleState(_stateMachine);
        PatrolState patrolState = new PatrolState(this, patrolData, _stateMachine);

        _stateMachine.RegisterState(PoliceStates.Idle, idleState);
        _stateMachine.RegisterState(PoliceStates.Patrol, patrolState);


        _stateMachine.ChangeState(PoliceStates.Idle);
        //_stateMachine.ChangeState(PoliceStates.Patrol);
    }

    void Update()
    {


        _stateMachine.Update();
    }

}

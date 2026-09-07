using UnityEngine;

public class FSMHunter : MonoBehaviour
{
    public float speed = 3f;
    private StateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = new StateMachine();

        //_stateMachine.RegisterState(PoliceStates.Idle, idleState);
    }
    private void Update()
    {
        _stateMachine.Update();
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;


public class StateMachine
{
    public State CurrentState { get; private set; }
    private Dictionary<Enum, State> _states = new Dictionary<Enum, State>();

    public void RegisterState(Enum stateType, State state)
    {
        /*if (!_states.ContainsKey(stateType))
        {
            _states.Add(stateType, state);
        }*/
        _states[stateType] = state;
    }

    public void ChangeState(Enum key)
    {
        State newState = _states[key];
        if (newState == CurrentState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }

}

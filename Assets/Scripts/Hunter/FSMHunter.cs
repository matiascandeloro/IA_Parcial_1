using System;
using UnityEngine;
using System.Collections.Generic;

public enum HunterStates
{
    Idle,
    Patrol,
    Attack,
    Gather
}

public class FSMHunter : Agent
{
    public float maxSpeed = 3f;

    public float _maxSteering = 3f;
    private StateMachine _stateMachine;

    private Agent _target;

    [SerializeField] private float _timeBetweenAttacks = 1f;
    [SerializeField] private float _rangeAttackRadius = 2f;
    [SerializeField] private float _meleeAttackRadius = 1f;

    [SerializeField] private PatrolData patrolData;

    [Header("Traps")]
    private bool _isBaitSet = false;
    [SerializeField] private GameObject _baitPrefab;
    [SerializeField] private float _timeBetweenBait = 5f;
    [SerializeField] private int _maxBaitCount = 5;
    private List<GameObject> _baitList = new List<GameObject>();
    private float _baitTimer = 0f;
    private int _baitCount = 0;
    


    private void Awake()
    {
        _stateMachine = new StateMachine();
        IdleState idleState = new IdleState(_stateMachine);
        PatrolState patrolState = new PatrolState(this, patrolData, _stateMachine);
        patrolState.SetPatrolLoopMode(true);
        //attack state
        //gather state

        //register states
        _stateMachine.RegisterState(HunterStates.Idle, idleState);
        _stateMachine.RegisterState(HunterStates.Patrol, patrolState);


        _stateMachine.ChangeState(HunterStates.Idle);
    }
    private void Update()
    {
        _stateMachine.Update();



        // Cebo, siempre controla el cooldown y si debe poner cebo, lo hace
        UpdateBaitTime();
        if (_isBaitSet)
        {
            setBait();
        }

     
    }

    private void UpdateBaitTime()
    {
        _baitTimer += Time.deltaTime;
    }

    private void setBait()
    {
        if (_baitTimer >= _timeBetweenBait)
        {
            _baitTimer = 0f;
            if (_baitList.Count <= _maxBaitCount)
            {
                _baitList.Add(Instantiate(_baitPrefab, transform.position, Quaternion.identity));
            }
        }
        //como los objetos se destruyen solos, se limpian de la lista para no tener referencias a null
        _baitList.RemoveAll(item => item == null);
    }

    public void setBaitMode(bool bait)
    {
        _isBaitSet = bait;
    }
}

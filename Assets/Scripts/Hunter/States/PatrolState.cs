using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PatrolState : State
{
    private FSMHunter _agent;
    private PatrolData _data;
    private int currentWaypointIndex;
    private int direction = 1;
    private bool _isLoop;

    public PatrolState(FSMHunter agent, PatrolData data,StateMachine stateMachine):base(stateMachine)
    {
        this._agent = agent;
        this._data = data;
    }

    public override void Enter()
    {
        _agent.setBaitMode(true);
    }
    public override void Exit()
    {
        _agent.setBaitMode(false);
    }
    public override void Update()
    {
        if (_isLoop)
            PatrolLoop();
        else
            PatrolPingPong();

    }

    private void PatrolLoop()
    {
        var nextWaypoint = _data.waypoints[currentWaypointIndex];

        if (Vector3.Distance(nextWaypoint.position, _data.transform.position) < _data.waypointCheckDistance)
        {
            currentWaypointIndex = currentWaypointIndex + 1 == _data.waypoints.Count ? 0 : currentWaypointIndex + 1;
        }
        //pasar a seek
        var dir = nextWaypoint.position - _data.transform.position;
        _data.transform.position += dir.normalized * _agent.maxSpeed * Time.deltaTime;
        //Seek(nextWaypoint.position);

    }
    /* revisaar no anda
    private Vector3 CalculateSteering(Vector3 desired)
    {
        Vector3 steering = desired - _agent.Velocity;

        steering = Vector3.ClampMagnitude(steering, _agent._maxSteering * Time.deltaTime);

        return steering;
    }

    private Vector3 DesiredVector(Vector3 target)
    {
        Vector3 desired = (target - _data.transform.position).normalized;
        desired *= _agent.maxSpeed;

        return desired;
    }

    private Vector3 Seek(Vector3 target)
    {
        return CalculateSteering(DesiredVector(target));
    }
    */
    private void PatrolPingPong()
    {
        var nextWaypoint = _data.waypoints[currentWaypointIndex];

        if (Vector3.Distance(nextWaypoint.position, _data.transform.position) < _data.waypointCheckDistance)
        {
            currentWaypointIndex += direction;
            if (currentWaypointIndex >= _data.waypoints.Count)
            {
                direction = -1;
                currentWaypointIndex = _data.waypoints.Count - 2;
            }
            else if (currentWaypointIndex < 0)
            {
                direction = 1;
                currentWaypointIndex = 0;
            }
        }
        //pasar a seek
        var dir = nextWaypoint.position - _data.transform.position;
        _data.transform.position += dir.normalized * _agent.maxSpeed * Time.deltaTime;
    }
    public void SetPatrolLoopMode(bool isLoop)
    {
        _isLoop = isLoop;
    }

}

[System.Serializable]
public class PatrolData
{
    public List<Transform> waypoints;
    public Transform transform;
    public float waypointCheckDistance;

}


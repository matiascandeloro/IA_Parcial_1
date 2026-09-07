using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class PatrolState : State
{
    private FSMAgent _agent;
    private PatrolData _data;
    private int currentWaypointIndex;
    private int direction = 1;
    private bool _isLoop;
    public PatrolState(FSMAgent agent, PatrolData data,StateMachine stateMachine):base(stateMachine)
    {
        this._agent = agent;
        this._data = data;
    }

    public override void Enter()
    {
        Debug.Log("Entering Patrol State");
    }
    public override void Exit()
    {
        Debug.Log("Exiting Patrol State");
    }
    public override void Update()
    {
        //if (_isLoop)
            PatrolLoop();
        //else
        //    PatrolPingPong();
        Debug.Log("Updating Patrol State");
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
        _data.transform.position += dir.normalized * _agent.speed * Time.deltaTime;

    }

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
        _data.transform.position += dir.normalized * _agent.speed * Time.deltaTime;
    }

}

[System.Serializable]
public class PatrolData
{
    public List<Transform> waypoints;
    public Transform transform;
    public float waypointCheckDistance;

}


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdvanceAgent : Agent
{
    [SerializeField] private bool drawGizmos;

    [Header("Stats")]
    [SerializeField] private float _maxSpeed = 3f;
    [SerializeField] private float _maxSteering = 3f;
    [SerializeField] private float _slowingDistance = 3f;
    [SerializeField] private float _minDistance = 0.1f;
    private bool isDead = false;
    private bool isBeingCollected = false;
    private bool _isBaiting;
    private bool _isEscaping;

    [Header("Agents")]
    [SerializeField] private LayerMask _agentPreyLayer;
    [SerializeField] private LayerMask _agentHunterLayer;
    [SerializeField] private LayerMask _agentBaitLayer;
    private static List<Agent> allAgents= new List<Agent>(); 

    [Header("Flocking")]

    [SerializeField] private float _separationRadius = 2f;
    [SerializeField] private float _alignmentRadius = 5f;
    [SerializeField] private float _cohesionRadius = 5f;
    [SerializeField, Range(0f,1f)] private float _separationWeight = 1f;
    [SerializeField, Range(0f,1f)] private float _alignmentWeight = 1f;
    [SerializeField, Range(0f,1f)] private float _cohesionWeight = 1f;

    [Header("References")]
    [SerializeField] private FSMHunter _target;

    public enum SteeringModes {Seek,Flee,Arrive,Pursuit,Evade,Flocking}
    public SteeringModes currentSteering;

    private void Awake()
    {
        //allAgents.Add(this);
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        _velocity = randomDirection.normalized * _maxSpeed;
    }

    void Update()
    {
        if (isDead)
        {
            _velocity = Vector3.zero;
            return;
        }

        _velocity += SteeringVector();
        transform.position += _velocity * Time.deltaTime;

        if (_velocity != Vector3.zero)
            transform.forward = _velocity;

        transform.position = Bounds.Instance.OutOfBounds(transform.position);
    }

    private Vector3 SteeringVector()
    {
        if (_target == null)
        {
            return Vector3.zero;  
        }
            
        switch (currentSteering)
        {
            case SteeringModes.Seek:
                return Seek(_target.transform.position);
            case SteeringModes.Flee:
                return Flee(_target.transform.position);
            case SteeringModes.Arrive:
                return Arrive(_target.transform.position);
            case SteeringModes.Pursuit:
                return Pursuit(_target);
            case SteeringModes.Evade:
                return Evade(_target);
            case SteeringModes.Flocking:
                return Flocking();
            default:
                return Vector3.zero;
        }
    }
    private Vector3 Flocking()
    {
        return CalculateSeparation(allAgents, _separationRadius) * _separationWeight
                 + CalculateAlignment(allAgents, _alignmentRadius) * _alignmentWeight
                 + CalculateCohesion(allAgents, _cohesionRadius) * _cohesionWeight;
    }
    
    private Vector3 CalculateSeparation(IEnumerable<Agent> list, float radius)
    {
        Vector3 dessired = default;
        int count = 0;

        foreach (var item in list)
        {
            if (item == this) continue;

            if (InRange(item.transform.position, radius))
            {
                dessired += (item.transform.position - transform.position);
                count++;
            }
        }

        if (count == 0) return Vector3.zero;
        dessired /= count;

        return CalculateSteering(-dessired.normalized * _maxSpeed);
    }
    
    private Vector3 CalculateAlignment(IEnumerable<Agent> list, float radius)
    {
        Vector3 desired = default;
        int count = 0;

        foreach (var item in list)
        {
            if (item == this) continue;

            if (InRange(item.transform.position, radius))
            {
                desired += item.Velocity;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;
        desired /= count;

        return CalculateSteering(desired.normalized * _maxSpeed);
    }

    private Vector3 CalculateCohesion(IEnumerable<Agent> list, float radius)
    {
        Vector3 desired = default;
        int count = 0;

        foreach (var item in list)
        {
            if (item == this) continue;

            if (InRange(item.transform.position, radius))
            {
                desired += item.transform.position;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;
        desired /= count;

        return Seek(desired);
    }

    private bool InRange(Vector3 pos, float radius) => (pos - transform.position).sqrMagnitude <= radius * radius;

    private Vector3 CalculateSteering(Vector3 desired)
    {
        Vector3 steering = desired - _velocity;

        steering = Vector3.ClampMagnitude(steering, _maxSteering * Time.deltaTime);

        return steering;
    }

    private Vector3 DesiredVector(Vector3 target)
    {
        Vector3 desired = (target - transform.position).normalized;
        desired *= _maxSpeed;

        return desired;
    }

    private Vector3 Seek(Vector3 target)
    {
        var desired = DesiredVector(target);

        return CalculateSteering(desired);
    }

    private Vector3 Flee(Vector3 target)
    {
        var desired = DesiredVector(target);

        return CalculateSteering(-desired);
    }

    private Vector3 Arrive(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        float distance = direction.magnitude;

        if (distance < _minDistance)
            return Vector3.zero;

        float targetSpeed = _maxSpeed * (distance / _slowingDistance);
        float desiredSpeed = Mathf.Min(targetSpeed, _maxSpeed);

        Vector3 desired = direction.normalized * desiredSpeed;
        Vector3 steering = CalculateSteering(desired);

        return CalculateSteering(desired);
    }

    private Vector3 CalculateFuture(Agent target)
    {
        Vector3 direccion = target.transform.position - transform.position;

        float distance = direccion.magnitude;
        var prediction = distance / (_maxSpeed + target.Velocity.magnitude);

        Vector3 futurePosition = target.transform.position + target.Velocity * prediction;

        return futurePosition;
    }

    private Vector3 Pursuit(Agent target)
    {
        var futurePosition = CalculateFuture(target);

        return Seek(futurePosition);
    }

    private Vector3 Evade(Agent target)
    {
        var futurePosition = CalculateFuture(target);

        return Flee(futurePosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_agentPreyLayer==(1 << other.gameObject.layer))
        { 
            Agent agent = other.gameObject.GetComponent<Agent>();

            if (agent == null) return;

            if (!allAgents.Contains(agent))
                allAgents.Add(agent);
        }

        if (_agentHunterLayer == (1 << other.gameObject.layer))
        {
            //_isEscaping = true;
            _target = other.gameObject.GetComponent<FSMHunter>();
            currentSteering = SteeringModes.Evade;
        }
        /*
        if (_agentBaitLayer == (1 << other.gameObject.layer))
        {
            _isBaiting = true;
            _target = other.gameObject.GetComponent<Bait>();
            currentSteering = SteeringModes.Arrive;
        }*/
    }
    private void OnTriggerExit(Collider other)
    {
        if (_agentPreyLayer==(1 << other.gameObject.layer))
        {
            Agent agent = other.gameObject.GetComponent<Agent>();

            if (agent == null) return;

                if (allAgents.Contains(agent))
                    allAgents.Remove(agent);

        }
        if (_agentHunterLayer == (1 << other.gameObject.layer))
        {
            _isEscaping = false;
            /*if (_target == null)
            {
                _isBaiting = false;*/
            currentSteering = SteeringModes.Flocking;
            /*}
            else
                currentSteering = SteeringModes.Arrive;
            */
        }
    }

    private void Die()
    {
        isDead = true;

        _velocity = Vector3.zero;
        Debug.Log(name + " fue eliminado.");
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void Collect()
    {
        if (isBeingCollected)
            return;

        isBeingCollected = true;
        
        gameObject.SetActive(false);

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        Vector3 randomPosition = Bounds.Instance.GetRandomPosition();

        transform.position = randomPosition;

        isDead = false;
        isBeingCollected = false;

        gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        SphereCollider _sphereCollider = gameObject.GetComponent<SphereCollider>();
        if (!drawGizmos) return;
        if (!_sphereCollider) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _sphereCollider.radius);
    }
}

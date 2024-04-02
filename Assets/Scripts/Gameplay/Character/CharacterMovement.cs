using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public bool isMoving = false;
    public bool isSlowed = false;
    public float baseSpeed = 1f; // make sure to set this in any controller which uses CharacterMovement
    //public float StoppingRange = 1.5f;

    public UnityEvent DestinationReached;
    protected virtual void OnDestinationReached() { DestinationReached?.Invoke(); }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public bool HasReachedDestination()
    {
        float dist = Vector3.Distance(transform.position, agent.destination);

        return dist <= agent.stoppingDistance;
    }
    public virtual void MoveTo(GameObject target, float stopRange)
    {
        MoveTo(target.transform.position, stopRange);
    }
    public virtual void MoveTo(Vector3 position)
    {
        MoveTo(position, agent.stoppingDistance);
    }
    public void MoveTo(Vector3 position, float stopRange)
    {
        if (agent.SetDestination(position))
        {
            agent.stoppingDistance = stopRange;
            agent.isStopped = false;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
    public void StopMoving()
    {
        isMoving = false;
        agent.isStopped = true; //persists, must switch back to false later
    }
    protected virtual void Update()
    {
        if (isMoving)
        {
            if (HasReachedDestination())
            {
                StopMoving();
                OnDestinationReached();
            }
        }
    }
    public void NoSpeed()
    {
        agent.speed = 0;
    }
    public void SlowSpeed()
    {
        agent.speed = baseSpeed / 2;
    }
    public void ResetSpeed()
    {
        agent.speed = baseSpeed;
    }
    // find a random walkable location on the nav mesh within a range
    public bool MoveToRandomLocation(float range)
    {
        // Generate a random direction and distance
        Vector3 randomDirection = Random.insideUnitSphere * range;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 targetPosition = Vector3.zero;

        // Try to find a nearby point on the NavMesh
        if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
            MoveTo(targetPosition, agent.stoppingDistance);
            return true;
        }

        return false;
    }
}

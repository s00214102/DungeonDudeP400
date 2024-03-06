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
    public float StoppingRange = 1.0f;

    public UnityEvent DestinationReached;
    protected virtual void OnDestinationReached() { DestinationReached?.Invoke(); }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public bool HasReachedDestination()
    {
        float dist = Vector3.Distance(transform.position, agent.destination);

        return dist <= StoppingRange;
    }

    public virtual void MoveTo(GameObject target, float stopRange)
    {
        MoveTo(target.transform.position, stopRange);
    }
    public void MoveTo(Vector3 position, float stopRange)
    {
        if (agent.SetDestination(position))
        {
            StoppingRange = stopRange;
            agent.isStopped = false;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    protected virtual void Update()
    {
        if (isMoving)
        {
            if (HasReachedDestination())
            {
                isMoving = false;
                agent.isStopped = true;//persists, must switch back to false later

                OnDestinationReached();
            }
        }
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
            MoveTo(targetPosition, StoppingRange);
            return true;
        }

        return false;
    }
}

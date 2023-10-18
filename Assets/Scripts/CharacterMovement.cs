using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterMovement : MonoBehaviour
{
    NavMeshAgent agent;
    protected bool isMoving = false;
    public float StoppingRange = 1.0f;

    public UnityEvent DestinationReached;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
       
    }

    bool HasReachedDestination()
    {
        float dist = Vector3.Distance(transform.position, agent.destination);

        return dist <= StoppingRange;
    }

    protected virtual void OnDestinationReached() { DestinationReached?.Invoke(); }

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
        if(isMoving)
        {
            if(HasReachedDestination())
            {
                isMoving = false;
                agent.isStopped = true;//persists, must switch back to false later

                OnDestinationReached();
            }
        }
    }
}

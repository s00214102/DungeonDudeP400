using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentDestination : ActionBase
{
    private NavMeshAgent _agent;
    public Transform target;
    protected override void OnInit()
    {
        _agent = Owner.GetComponent<NavMeshAgent>();
    }
    protected override TaskStatus OnUpdate()
    {
        _agent.SetDestination(target.position);
        return TaskStatus.Success;
    }
}

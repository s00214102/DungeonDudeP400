using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToGoalAction : ActionBase
{
    private CharacterMovement _movement;
    public Vector3 goalPosition;

    protected override void OnInit()
    {
        _movement = Owner.GetComponent<CharacterMovement>();
    }
    protected override TaskStatus OnUpdate()
    {
        // is the path not being generated, and are we not at the goal
        if (!_movement.agent.pathPending && GoalIsNotTheCurrentDestination())
        {
            _movement.MoveTo(goalPosition, 1);
        }

        return TaskStatus.Success;
    }
    private bool GoalIsNotTheCurrentDestination()
    {
        float dist = Vector3.Distance(_movement.agent.destination, goalPosition);

        return dist > 1;
    }

}

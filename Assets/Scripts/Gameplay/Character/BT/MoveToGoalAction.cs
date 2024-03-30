using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToGoalAction : ActionBase
{
    private CharacterMovement _movement;
    public Vector3 goalPosition;
    private BT_Hero_Controller _controller;
    protected override void OnInit()
    {
        _controller = Owner.GetComponent<BT_Hero_Controller>();
        _movement = Owner.GetComponent<CharacterMovement>();
    }
    protected override TaskStatus OnUpdate()
    {
        _controller.ChangeStateImage(0); // inefficient to do this each tick but its temporary
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

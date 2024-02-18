using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToEngageEnemy : ActionBase
{
    private CharacterMovement _movement;
    private EntityProximityDetectionBT _detection;
    private BT_Hero_Controller _controller;

    protected override void OnInit()
    {
        _movement = Owner.GetComponent<CharacterMovement>();
        _detection = Owner.GetComponent<EntityProximityDetectionBT>();
        _controller = Owner.GetComponent<BT_Hero_Controller>();
    }
    protected override TaskStatus OnUpdate()
    {
        // set the destination and begin moving
        // is the path not being generated, and are we not at the goal
        if (!_movement.agent.pathPending && EnemyIsNotTheCurrentDestination())
        {
            _movement.MoveTo(_detection.closestTarget.transform.position, _controller.HeroData.AttackRange);
        }

        return TaskStatus.Success;
    }
    private bool EnemyIsNotTheCurrentDestination()
    {
        float dist = Vector3.Distance(_movement.agent.destination, _detection.closestTarget.transform.position);

        return dist > 1;
    }

}

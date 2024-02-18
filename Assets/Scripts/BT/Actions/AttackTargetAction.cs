using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTargetAction : ActionBase
{
    BT_Hero_Controller _controller;
    protected override void OnInit()
    {
        _controller = Owner.GetComponent<BT_Hero_Controller>();
    }
    protected override TaskStatus OnUpdate()
    {
        _controller.ChangeStateImage(2); // inefficient to do this each tick but its temporary
        // update checks if we should keep attacking
        // if the target isnt dead then return success
        // if the target is dead then return failure
        if (_controller._detection.closestTarget.Health.isDead)
            return TaskStatus.Failure;

        AttackTarget();

        return TaskStatus.Success;
    }
    private void AttackTarget()
    {
        Health enemyHealth = _controller._detection.closestTarget.GetComponent<Health>();
        Debug.Log("AttackTargetAction - Dealing damage to enemy");
        if (enemyHealth != null)
            enemyHealth.TakeDamage(_controller.HeroData.Damage);
        else
        {
            Debug.Log("AttackTargetAction - Enemy Health component not found.");
            //StopAttacking();
        }
    }
}

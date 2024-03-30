using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Engage_State : BaseState
{
    HeroController controller;
    public Hero_Engage_State(HeroController controller) : base(controller)
    {
        this.controller = controller;
    }
    public override void Enter()
    {
        Debug.Log("Entered engage state");
        if (controller.Target != null)
        {
            controller.CharacterMovement.MoveTo(controller.Target.position, controller.HeroData.AttackRange);
            controller.CharacterMovement.DestinationReached.AddListener(OnEnemyReached);
        }
        else
            controller.SetState((int)HeroState.Explore);
    }

    private void OnEnemyReached()
    {
        controller.SetState((int)HeroState.Attack);
    }
    public override void Exit()
    {
        controller.CharacterMovement.DestinationReached.RemoveListener(OnEnemyReached);
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }

    Transform FindClosestTarget()
    {
        GameObject[] covers = GameObject.FindGameObjectsWithTag("Play");
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject go in covers)
        {
            float dist = Vector3.Distance(go.transform.position, controller.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = go.transform;
            }
        }
        covers = null;
        return closest;
    }
}

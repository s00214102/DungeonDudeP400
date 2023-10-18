using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Attack_State : BaseState
{
    HeroController controller;
    Health enemy;
    public Hero_Attack_State(HeroController controller) : base(controller)
    {
        this.controller = controller;
    }
    public override void Enter()
    {
        Debug.Log("Entered attack state");
        enemy = controller.Target.GetComponent<Health>();
        enemy.EntityDied.AddListener(StopAttacking);
        controller.StartAttacking();
    }
    private void StopAttacking()
    {
        controller.StopAttacking();
        controller.SetState((int)HeroState.Explore);
    }
    public override void Exit()
    {
        enemy.EntityDied.RemoveListener(StopAttacking);
        base.Exit();
    }

    public override void Update()
    {
        //if(enemy == null)
        //    controller.SetState((int)HeroState.Explore);
    }

    Transform FindClosestTarget()
    {
        GameObject[] covers = GameObject.FindGameObjectsWithTag("Sleep");
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

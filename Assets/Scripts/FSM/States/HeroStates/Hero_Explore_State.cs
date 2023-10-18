using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Explore_State : BaseState
{
    HeroController controller;
    Transform goal;
    public Hero_Explore_State(HeroController controller) : base(controller)
    {
        this.controller = controller;
    }
    public override void Enter()
    {
        Debug.Log("Entered explore state");

        goal = GameObject.Find("Goal").transform;

        controller.Target = null;
        controller.Detection.StartSearch();
        controller.Detection.TargetFound.AddListener(FoundEnemy);

        if (goal != null)
            controller.CharacterMovement.MoveTo(goal.position, 1);
        else
            Debug.LogWarning("Hero couldnt find a goal while in explore state.");
    }
    private void FoundEnemy(Transform enemy)
    {
        controller.Target = enemy;
        controller.SetState((int)HeroState.Engage);
    }
    public override void Exit()
    {
        controller.Detection.TargetFound.RemoveListener(FoundEnemy);
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }

    Transform FindClosestTarget()
    {
        GameObject[] covers = GameObject.FindGameObjectsWithTag("Eat");
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

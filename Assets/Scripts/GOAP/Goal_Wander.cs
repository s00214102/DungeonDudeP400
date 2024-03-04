using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// wander around if stationary for a length of time
public class Goal_Wander : Goal_Base
{
    [SerializeField] int minPriority = 0;
    [SerializeField] int maxPriority = 30;
    [SerializeField] float priorityBuildRate = 1f;
    [SerializeField] float priorityDecayRate = 0.1f;
    float currentPriority = 0f;

    public override int OnCalculatePriority()
    {
        return Mathf.FloorToInt(currentPriority);
    }

    public override void OnTickGoal()
    {
        if (movement.isMoving)
            currentPriority -= priorityDecayRate * Time.deltaTime;
        else
            currentPriority += priorityBuildRate * Time.deltaTime;
    }

    public override void OnGoalActivated()
    {
        currentPriority = maxPriority;
    }

    public override bool CanRun()
    {
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// wander around if stationary for a length of time
public class Goal_Wander : Goal_Base
{
    [SerializeField] int minPriority = 0;
    [SerializeField] int maxPriority = 12;
    [SerializeField] float priorityBuildRate = 1f;
    [SerializeField] float priorityDecayRate = 0.5f;
    float currentPriority = 0f;

    public override int OnCalculatePriority()
    {
        return Mathf.FloorToInt(currentPriority);
    }

    public override void OnTickGoal()
    {
        if (movement.isMoving && currentPriority > 0)
            currentPriority -= priorityDecayRate * Time.deltaTime;
        else if (currentPriority < maxPriority)
            currentPriority += priorityBuildRate * Time.deltaTime;
    }

    public override void OnGoalActivated(Action_Base _linkedAction)
    {
        base.OnGoalActivated(_linkedAction);
        currentPriority = maxPriority;
    }

    public override bool CanRun()
    {
        return true;
    }
    public override string ToString()
    {
        return $"Wander priority: {currentPriority}";
    }
}

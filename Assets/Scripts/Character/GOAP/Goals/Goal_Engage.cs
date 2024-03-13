using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Engage : Goal_Base
{
    private int EngagePriority = 60;
    private int currentPriority = 0;

    public override int OnCalculatePriority()
    {
        return currentPriority;
    }

    public override void OnTickGoal()
    {
        currentPriority = 0;
        if (knowledge.closestTarget != null)
            currentPriority = EngagePriority;
    }

    public override bool CanRun()
    {
        if (knowledge.closestTarget == null)
            return false;
        return true;
    }
    public override string ToString()
    {
        return $"Engage priority: {currentPriority}";
    }
}

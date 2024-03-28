using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Engage : Goal_Base
{
    private int Priority = 0;

    public override int OnCalculatePriority()
    {
        return Priority;
    }

    public override void OnTickGoal()
    {
        Priority = 0;

        if (knowledge.closestTarget != null)
        {
            switch (data.HeroTraits.Aggression)
            {
                case 0:
                    Priority = 10;
                    break;
                case 1:
                    Priority = 30;
                    break;
                case 2:
                    Priority = 70;
                    break;
                case 3:
                    Priority = 999;
                    break;
                default:
                    Priority = 0;
                    break;
            }
        }
    }

    public override bool CanRun()
    {
        if (knowledge.closestTarget == null)
            return false;
        return true;
    }
    public override string ToString()
    {
        return $"Engage priority: {Priority}";
    }
}

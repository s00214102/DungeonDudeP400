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

        var result = knowledge.RecallHighestAlertEnemy();
        if (result.found)
        {
            switch (data.HeroTraits.Aggression)
            {
                case 0:
                    Priority = 20;
                    break;
                case 1:
                    Priority = 45;
                    break;
                case 2:
                    Priority = 70;
                    break;
                case 3:
                    Priority = 200;
                    break;
                default:
                    Priority = 0;
                    break;
            }
        }
    }

    public override bool CanRun()
    {
        var result = knowledge.RecallHighestAlertEnemy();
        if (!result.found)
            return false;
        return true;
    }
    public override string ToString()
    {
        return $"Engage priority: {Priority}";
    }
}

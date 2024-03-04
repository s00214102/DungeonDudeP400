using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Idle : Goal_Base
{
    [SerializeField] int Priority = 10; // idle simply has a fixed priority that we choose
    public virtual int OnCalculatePriority()
    {
        return Priority;
    }
    public virtual bool CanRun()
    {
        return true;
    }
}

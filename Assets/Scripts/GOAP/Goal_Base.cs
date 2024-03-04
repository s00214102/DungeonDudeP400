using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGoal
{
    int OnCalculatePriority();
    bool CanRun();
    void OnTickGoal();
    void OnGoalActivated();
    void OnGoalDeactivated();
}

public class Goal_Base : MonoBehaviour, IGoal
{
    protected CharacterMovement movement;
    protected EntityProximityDetection detection;

    private void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        detection = GetComponent<EntityProximityDetection>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        OnTickGoal();
    }

    public virtual bool CanRun()
    {
        throw new System.NotImplementedException();
    }

    public virtual int OnCalculatePriority()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnGoalActivated()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnGoalDeactivated()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnTickGoal()
    {
        throw new System.NotImplementedException();
    }
}

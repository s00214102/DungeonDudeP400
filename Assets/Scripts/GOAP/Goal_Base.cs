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
    protected EntityProximityDetectionBT detection;

    private void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        detection = GetComponent<EntityProximityDetectionBT>();
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
        return false;
    }

    public virtual int OnCalculatePriority()
    {
        return 0;
    }

    public virtual void OnGoalActivated()
    {

    }

    public virtual void OnGoalDeactivated()
    {

    }

    public virtual void OnTickGoal()
    {

    }
}

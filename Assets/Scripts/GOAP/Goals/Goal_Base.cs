using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGoal
{
    int OnCalculatePriority();
    bool CanRun();
    void OnTickGoal();
    void OnGoalActivated(Action_Base _LinkedAction);
    void OnGoalDeactivated();
}

public class Goal_Base : MonoBehaviour, IGoal
{
    protected CharacterMovement movement;
    protected EntityProximityDetectionBT detection;
    protected Action_Base LinkedAction;
    protected GOAP_Hero_Data data;

    private void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        detection = GetComponent<EntityProximityDetectionBT>();
        data = GetComponent<GOAP_Hero_Data>();
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

    public virtual void OnGoalActivated(Action_Base _LinkedAction)
    {
        LinkedAction = _LinkedAction;
    }

    public virtual void OnGoalDeactivated()
    {
        LinkedAction = null;
    }

    public virtual void OnTickGoal()
    {

    }
}

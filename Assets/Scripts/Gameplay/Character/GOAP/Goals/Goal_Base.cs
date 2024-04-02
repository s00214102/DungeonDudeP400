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
    protected HeroProximityDetection detection;
    protected HeroKnowledge knowledge;
    protected Health health;
    protected HeroStatus status;
    protected Action_Base LinkedAction;
    protected GOAP_Hero_Data data;
    protected GOAP_Planner planner;

    private void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        detection = GetComponent<HeroProximityDetection>();
        knowledge = GetComponent<HeroKnowledge>();
        data = GetComponent<GOAP_Hero_Data>();
        health = GetComponent<Health>();
        status = GetComponent<HeroStatus>();
        planner = GetComponent<GOAP_Planner>();
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

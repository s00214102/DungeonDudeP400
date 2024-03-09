using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EntityProximityDetection))]
// simple script for stationary monsters which attack a hero when theyre in range
public class SimpleMonster : BaseStateMachine
{
    private enum MobState
    {
        Search, // search for a target to attack
        Attack // close enough to target, attacking repeatedly
    }
    private MobState CurrentState;
    private Health health;
    private EntityProximityDetection detection;
    private Transform Target;
    private Dictionary<MobState, BaseState> States = new Dictionary<MobState, BaseState>();
    private FSM_Manager _manager;
    private bool _managed = false;

    private void Awake()
    {
        health = GetComponent<Health>();
        detection = GetComponent<EntityProximityDetection>();

        _manager = GetComponentInParent<FSM_Manager>();
        if (_manager != null)
        {
            _managed = true;
            _manager.AddHero(this.gameObject);
        }
        else
            _managed = false;
    }
    void Start()
    {
        InitiliazeStates();

        SetState(0);
    }
    public void ManagerUpdate()
    {
        if (_managed)
        {
            if (CurrentImplimentation != null)
                CurrentImplimentation.Update();
        }

    }
    private void Update()
    {
        if (!_managed)
        {
            if (CurrentImplimentation != null)
                CurrentImplimentation.Update();
        }
    }
    private void InitiliazeStates()
    {
        //States.Add(HeroState.Explore, new Hero_Explore_State(this));
    }
    public override void SetState(int newState)
    {
        CurrentState = (MobState)newState;

        if (States.ContainsKey(CurrentState))
        {
            // if another state is running, exit before switching to new state
            if (CurrentImplimentation != null)
                CurrentImplimentation.Exit();

            CurrentImplimentation = States[CurrentState];
            CurrentImplimentation.Enter();
        }

        base.SetState(newState);
    }
}

public class SimpleAttackMob_Attack_State : BaseState
{
    HeroController controller;
    Health enemy;
    public SimpleAttackMob_Attack_State(HeroController controller) : base(controller)
    {
        this.controller = controller;
    }
    public override void Enter()
    {
        Debug.Log("Entered attack state");
        enemy = controller.Target.GetComponent<Health>();
        enemy.EntityDied.AddListener(StopAttacking);
        controller.StartAttacking();
    }
    private void StopAttacking()
    {
        controller.StopAttacking();
        controller.SetState((int)HeroState.Explore);
    }
    public override void Exit()
    {
        enemy.EntityDied.RemoveListener(StopAttacking);
        base.Exit();
    }
}

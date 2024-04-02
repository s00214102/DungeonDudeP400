using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EntityProximityDetection))]
[RequireComponent(typeof(Attack))]
// simple script for stationary monsters which attack a hero when theyre in range
public class MonsterFSM : BaseStateMachine
{
    private enum MobState
    {
        Search, // search for a target to attack
        Engage, // move to the target
        Attack // close enough to target, attacking repeatedly
    }
    private MobState CurrentState;
    private Health health;
    internal Attack attack;
    private EntityProximityDetection detection;
    internal Transform Target;
    private Dictionary<MobState, BaseState> States = new Dictionary<MobState, BaseState>();
    private FSM_Manager _manager;
    private bool _managed = false;

    private void Awake()
    {
        health = GetComponent<Health>();
        detection = GetComponent<EntityProximityDetection>();
        attack = GetComponent<Attack>();

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
        States.Add(MobState.Attack, new SimpleAttackMob_Attack_State(this));
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
    public void StartInvoke(string methodName, float repeatRate)
    {
        InvokeRepeating(methodName, 0, repeatRate);
    }
}

public class SimpleAttackMob_Attack_State : BaseState
{
    Health targetHealth;
    MonsterFSM controller;
    public SimpleAttackMob_Attack_State(MonsterFSM controller) : base(controller)
    {
        this.controller = controller;
    }
    public override void Enter()
    {
        controller.attack.StartAttacking();
    }
    public override void Exit()
    {
        controller.attack.StopAttacking();
        base.Exit();
    }
}

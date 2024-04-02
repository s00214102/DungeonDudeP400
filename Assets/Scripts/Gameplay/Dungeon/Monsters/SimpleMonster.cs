using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EntityProximityDetection))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(Attack))]
[RequireComponent(typeof(MonsterData))]
// simple script for stationary monsters which attack a hero when theyre in range
public class SimpleMonster : MonoBehaviour
{
    private enum MobState
    {
        Search, // search for a target to attack
        Engage, // move towards target
        Attack // close enough to target, attacking repeatedly
    }
    [SerializeField] private MobState currentState;
    private Health health;
    private EntityProximityDetection detection;
    private CharacterMovement movement;
    private Attack attack;
    private MonsterData data;

    private FSM_Manager _manager;
    private bool _managed = false;

    private void Awake()
    {
        health = GetComponent<Health>();
        detection = GetComponent<EntityProximityDetection>();
        attack = GetComponent<Attack>();
        movement = GetComponent<CharacterMovement>();
        data = GetComponent<MonsterData>();

        movement.baseSpeed = data.Speed;
        health.SetMaxHealth(data.Health);

        // _manager = GetComponentInParent<FSM_Manager>();
        // if (_manager != null)
        // {
        //     _managed = true;
        //     _manager.AddHero(this.gameObject);
        // }
        // else
        //     _managed = false;
    }
    void Start()
    {
        currentState = 0;
    }
    // public void ManagerUpdate()
    // {
    //     if (_managed)
    //     {

    //     }

    // }

    // simple AI using an Enum and switch statement
    // 'Update' methods are run via the switch statement / Update method
    // 'Enter' methods are used to run code once when entering a new state
    private void Update()
    {
        SimpleStateMachine();
    }
    private void SimpleStateMachine()
    {
        // run the current states logic in Update
        switch (currentState)
        {
            case MobState.Search:
                SearchStateUpdate();
                break;
            case MobState.Engage:
                EngageStateUpdate();
                break;
            case MobState.Attack:
                AttackStateUpdate();
                break;
        }
    }
    private void EnterSearchState()
    {
        attack.OnFinishedAttacking.RemoveListener(EnterSearchState);
        currentState = MobState.Search;
    }
    private void SearchStateUpdate()
    {
        // simply search for targets, and move to engage state when one is found
        if (detection.closestTarget != null)
            EnterEngageState();
    }
    private void EnterEngageState()
    {
        if (detection.closestTarget == null)
        {
            movement.StopMoving();
            EnterSearchState();
        }
        //movement.MoveTo(detection.closestTarget.transform.position);
        currentState = MobState.Engage;
    }
    private void EngageStateUpdate()
    {
        if (detection.closestTarget != null)
        {
            movement.agent.SetDestination(detection.closestTarget.transform.position);
            // if we got to our destination and are still in engage state, reenter engage state
            if (!movement.isMoving)
                movement.MoveTo(detection.closestTarget.transform.position);
        }
        else
        {
            movement.StopMoving();
            EnterSearchState();
        }
        // should enter attack state when close enough
        if (InAttackRange())
        {
            movement.StopMoving();
            EnterAttackState();
        }
    }
    private void EnterAttackState()
    {
        attack.StartAttacking();
        attack.OnFinishedAttacking.AddListener(EnterSearchState);
        currentState = MobState.Attack;
    }
    private void AttackStateUpdate()
    {
        // attacking is handled by the attack component
        if (detection.closestTarget == null || !InAttackRange())
        {
            attack.StopAttacking();
            EnterSearchState();
        }
        else
            transform.LookAt(detection.closestTarget.transform);
    }
    private bool InAttackRange()
    {
        float dist = Vector3.Distance(transform.position, detection.closestTarget.transform.position);
        return dist <= data.AttackRange;
    }
}

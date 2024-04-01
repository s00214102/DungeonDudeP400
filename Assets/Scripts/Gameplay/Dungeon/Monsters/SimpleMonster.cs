using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EntityProximityDetection))]
// simple script for stationary monsters which attack a hero when theyre in range
public class SimpleMonster : MonoBehaviour
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

    }
    public void ManagerUpdate()
    {
        if (_managed)
        {

        }

    }
    private void Update()
    {
        if (!_managed)
        {

        }
    }

}

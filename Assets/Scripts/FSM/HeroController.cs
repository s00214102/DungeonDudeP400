using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[System.Serializable]
public enum HeroState
{
    Explore, // moves through the dungeon
    Engage, // enemy was detected, moving closer
    Attack, // close enough to enemy, attacking repeatedly
    Death, // died in combat, playing death animation etc
    Celebrate // won in combat, no other enemies nearby, having a quick celebration
}
public class HeroController : BaseStateMachine
{
    private NavMeshAgent agent;

    public HeroState CurrentState;
    public HeroData HeroData;
    Dictionary<HeroState, BaseState> States = new Dictionary<HeroState, BaseState>();
    [HideInInspector] public CharacterMovement CharacterMovement;
    [HideInInspector] public EntityProximityDetection Detection;
    [SerializeField] Sprite[] StateImages = new Sprite[4];
    [SerializeField] Image StateImage;
    private FSM_Hero_Manager _manager;
    private bool _managed = false;

    //[HideInInspector] 
    public Transform Target; // set when a target is detected in explore, used to move to in engage, used to attack in attack

    private void Awake()
    {
        _manager = GetComponentInParent<FSM_Hero_Manager>();
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
        CharacterMovement = GetComponent<CharacterMovement>();
        agent = GetComponent<NavMeshAgent>();
        Detection = GetComponent<EntityProximityDetection>();

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

    #region StateMachine
    private void InitiliazeStates()
    {
        States.Add(HeroState.Explore, new Hero_Explore_State(this));
        States.Add(HeroState.Engage, new Hero_Engage_State(this));
        States.Add(HeroState.Attack, new Hero_Attack_State(this));
        States.Add(HeroState.Death, new Hero_Death_State(this));
    }
    public override void SetState(int newState)
    {
        CurrentState = (HeroState)newState;

        if (States.ContainsKey(CurrentState))
        {
            // if another state is running, exit before switching to new state
            if (CurrentImplimentation != null)
                CurrentImplimentation.Exit();

            CurrentImplimentation = States[CurrentState];
            CurrentImplimentation.Enter();
            ChangeStateImage(newState);
        }

        base.SetState(newState);
    }
    public void ChangeStateImage(int state)
    {
        StateImage.sprite = StateImages[state];
    }
    #endregion

    #region Attack
    // XXX swap to using a generic Health class which every character has, makes dealing damage to anything easy
    private Health enemy;
    public void StartAttacking()
    {
        enemy = Target.GetComponent<Health>();
        //StartCoroutine("AttackTarget");
        InvokeRepeating("AttackTarget",0,HeroData.AttackRate);
    }
    public void StopAttacking()
    {
        //StopCoroutine("AttackTarget");
        CancelInvoke("AttackTarget");
    }
    private void AttackTarget()
    {
        if (enemy != null)
            enemy.TakeDamage(HeroData.Damage);
        else
            Debug.Log("Enemy not found, nothing to attack.");
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using UnityEngine.AI;
using UnityEngine.UI;

public class BT_Hero_Controller : MonoBehaviour
{
    [SerializeField]
    private BehaviorTree _tree;
    private NavMeshAgent _agent;
    private Animator _animator;
    private CharacterMovement _movement;
    [HideInInspector] public EntityProximityDetectionBT _detection;

    public HeroData HeroData;
    [SerializeField] Sprite[] StateImages = new Sprite[4];
    [SerializeField] Image StateImage;
    Vector3 goal;
    private void Awake()
    {
        if(HeroData == null)
        {
            Debug.Log($"HeroData missing for {gameObject.name}.");
            return;
        }
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _movement = GetComponent<CharacterMovement>();
        _detection = GetComponent<EntityProximityDetectionBT>();

        goal = GameObject.Find("Goal").transform.position;

        _tree = new BehaviorTreeBuilder(gameObject)
            .Selector("Root Selector")

                .RepeatUntilFailure()
                    .Sequence("Engage Enemy")
                        .Condition("Is the Enemy Detected?", IsEnemyDetected)
                        .MoveToEngageEnemy()
                        .Condition("In Attack Range?", () => !IsEnemyInAttackRange())
                    .End()
                .End()

                .RepeatUntilFailure()
                    .Sequence("Attack Enemy")
                        .Condition("In Attack Range?", IsEnemyInAttackRange)
                        .Condition("Is the Enemy Alive?", IsEnemyAlive)
                        .AttackTarget()
                        .WaitTime(HeroData.AttackRate)
                    .End()
                .End()

                .Sequence("Find Goal")
                    .Condition("Is Goal Not Reached", IsGoalNotReached)
                    .MoveToGoal(goal)
                .End()
                .Sequence("Celebrate")
                    .Condition("Is Goal Reached", IsGoalReached)
                    .Do("Play Celebration Animation", PlayCelebrationAnimation)
                .End()
            .End()
        .Build();

        // Runs all child nodes at the same time until they all return Success.
        // Exits and stops all running nodes if ANY of them return Failure.
        //.Parallel()
        //// Both of these tasks will run every frame
        //.Do(() => { return TaskStatus.Continue; })
        //.Do(() => { return TaskStatus.Continue; })
        //.End()
    }
    private void Update()
    {
        _tree.Tick();
    }
    private bool IsGoalReached()
    {
        float dist = Vector3.Distance(transform.position, goal);
        return dist <= _movement.StoppingRange;
    }
    private bool IsGoalNotReached()
    {
        float dist = Vector3.Distance(transform.position, goal);
        return dist > _movement.StoppingRange;
    }
    private bool IsEnemyDetected()
    {
        if (_detection.closestTarget != null)
            return true;
        else
            return false;
    }
    private bool IsEnemyInAttackRange()
    {
        if (_detection.closestTarget == null)
            return false;

        float dist = Vector3.Distance(transform.position, _detection.closestTarget.transform.position);
        //Debug.Log($"Current distance: {dist} / {HeroData.AttackRange}");
        return dist <= HeroData.AttackRange;
    }
    private bool IsEnemyAlive()
    {
        return _detection.closestTarget.gameObject.activeInHierarchy;
    }
    private Health enemyHealth;
    private bool attacking = false;
    private TaskStatus AttackEnemy() 
    {
        //Debug.Log("Attacking target");
        // if not already attacking, start attacking
        //if (!_attack.attacking && _detection.closestTarget.gameObject.activeInHierarchy==true)
        //{
        //    Debug.Log("BT_Hero_Controller - StartAttacking");
        //    //_attack.StartAttacking(_detection.closestTarget);
        //}
        return TaskStatus.Success; 
    }
    private bool IsHealthLow() 
    { 
        /* Implement health check */ 
        return false; 
    }
    private TaskStatus NavigateToSafeZone() 
    { 
        /* Implement flee logic */ 
        return TaskStatus.Success; 
    }
    private TaskStatus PlayCelebrationAnimation() 
    {
        Debug.Log("CELEBRATION!");
        ChangeStateImage(4);
        return TaskStatus.Success; 
    }
    public void ChangeStateImage(int state)
    {
        if(StateImage!=null)
            StateImage.sprite = StateImages[state];
        else
            Debug.Log("Image not found, cant change state sprite. Make a reference to the Image in the Editor.");
    }
}

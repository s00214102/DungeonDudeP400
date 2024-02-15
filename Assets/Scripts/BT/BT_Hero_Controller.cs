using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using UnityEngine.AI;

public class BT_Hero_Controller : MonoBehaviour
{
    [SerializeField]
    private BehaviorTree _tree;
    private NavMeshAgent _agent;
    private Animator _animator;
    private CharacterMovement _movement;
    private EntityProximityDetection _detection;
    public HeroData HeroData;

    Vector3 goal;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _movement = GetComponent<CharacterMovement>();
        _detection = GetComponent<EntityProximityDetection>();

        goal = GameObject.Find("Goal").transform.position;

        // Priorities
        // Engage the enemy
        // Flee if low health
        // Find the goal
        // Celebrate

        //_tree = new BehaviorTreeBuilder(gameObject)
        //    .Selector("Root Selector")
        //        .Sequence("Engage Enemy")
        //            .Condition("Is Enemy Detected", IsEnemyDetected)
        //            .Do("Move To Enemy", MoveToEnemy)
        //            .Condition("Is In Attack Range", IsInAttackRange)
        //            .Do("Attack Enemy", AttackEnemy)
        //        .End()
        //        .Sequence("Flee")
        //            .Condition("Is Health Low", IsHealthLow)
        //            .Do("Navigate To Safe Zone", NavigateToSafeZone)
        //        .End()
        //        .Sequence("Find Goal")
        //            .Condition("Is Goal Not Reached", IsGoalNotReached)
        //            .Do("Move To Goal", MoveToGoal)
        //        .End()
        //        .Sequence("Celebrate")
        //            .Condition("Is Goal Reached", IsGoalReached)
        //            .Do("Play Celebration Animation", PlayCelebrationAnimation)
        //        .End()
        //    .End()
        //    .Build();

        _tree = new BehaviorTreeBuilder(gameObject)
            .Selector("Root Selector")

                .Sequence("Engage Enemy")
                    .Condition("Is Enemy Detected", IsEnemyDetected)
                    .MoveToGoal(_detection.Target.position)
                    .Condition("Is In Attack Range", IsInAttackRange)
                    .Do("Attack Enemy", AttackEnemy)
                .End()

                .Sequence("Celebrate")
                    .Condition("Is Goal Reached", IsGoalReached)
                    .Do("Play Celebration Animation", PlayCelebrationAnimation)
                .End()

                .Sequence("Find Goal")
                    .Condition("Is Goal Not Reached", IsGoalNotReached)
                    .MoveToGoal(goal)
                .End()
                
            .End()
        .Build();
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
        if (_detection.Target != null)
            return true;
        else
            return false;
    }
    private bool IsInAttackRange() 
    {
        float dist = Vector3.Distance(transform.position, _detection.Target.position);
        return dist <= 1;
    }
    private TaskStatus AttackEnemy() 
    {
        Debug.Log("Attacking target");
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
        return TaskStatus.Success; 
    }
}

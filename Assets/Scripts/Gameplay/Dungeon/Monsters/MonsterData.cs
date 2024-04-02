using UnityEngine;
using UnityEngine.AI;

// container component for monsters values used by other components like Health, Attack, etc.
public class MonsterData : MonoBehaviour
{
	public int Health = 10;
	public int Damage = 1;
	public float Speed = 4;
	public float AttackRate = 1f;
	public float AttackRange = 5f;
	public float SearchRange = 5f;
	public float StopDistance = 3f;

	NavMeshAgent navAgent;
	Health health;
	EntityProximityDetection detection;

	private void Awake()
	{
		navAgent = GetComponent<NavMeshAgent>();
		health = GetComponent<Health>();
		detection = GetComponent<EntityProximityDetection>();
	}
	private void Start()
	{
		if (navAgent != null)
		{
			navAgent.speed = Speed;
			navAgent.stoppingDistance = StopDistance;
		}

		if (health != null)
			health.SetMaxHealth(Health);

		if (detection != null)
			detection.range = SearchRange;
	}
}
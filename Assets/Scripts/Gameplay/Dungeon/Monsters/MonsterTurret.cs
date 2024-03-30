using UnityEngine;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EntityProximityDetection))]
public class MonsterTurret : MonoBehaviour
{
	EntityProximityDetection detection;
	Health health;

	[SerializeField] int attackRange = 2;
	[SerializeField] int attackDamage = 1;
	[SerializeField] int attackRate = 1;

	private void Awake()
	{
		detection = GetComponent<EntityProximityDetection>();
		health = GetComponent<Health>();
	}
	private void Start()
	{
		// detection component used to get targets
		// if closestTarget is not null, check if it is in range to attack
		InvokeRepeating("AttackTarget", attackRate, attackRate);
	}
	private void AttackTarget()
	{
		if (detection.closestTarget == null)
			return;
		if (health.isDead)
			return;
		if (Helper.InRange(this.transform.position, detection.closestTarget.transform.position, attackRange))
		{
			Health targetHealth;
			if (!detection.closestTarget.TryGetComponent<Health>(out targetHealth))
				return;
			if (!targetHealth.isDead)
				targetHealth.TakeDamage(attackDamage);
		}
	}
}
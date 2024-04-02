using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EntityProximityDetection))]
[RequireComponent(typeof(MonsterData))]
public class MonsterTurret : MonoBehaviour
{
	EntityProximityDetection detection;
	Health health;
	MonsterData data;
	public UnityEvent OnAttack;
	public UnityEvent OnFinishedAttacking;

	float attackRange = 2;
	int attackDamage = 1;
	float attackRate = 1;

	public GameObject projectilePrefab;
	public float projectileMoveSpeed = 5f;
	public Transform projectileOrigin;

	private void Awake()
	{
		detection = GetComponent<EntityProximityDetection>();
		health = GetComponent<Health>();
		data = GetComponent<MonsterData>();

		attackRange = data.AttackRange;
		attackDamage = data.Damage;
		attackRate = data.AttackRate;
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
			{
				OnAttack?.Invoke();

				StartCoroutine(ShootProjectile(detection.closestTarget.transform.position));

				//transform.LookAt(detection.closestTarget.transform);
				Vector3 direction = detection.closestTarget.transform.position - transform.position;
				direction.y = 0;
				transform.rotation = Quaternion.LookRotation(direction);

				targetHealth.TakeDamage(attackDamage);
				OnFinishedAttacking?.Invoke();
			}
		}
	}
	// Coroutine to move the sphere from its current position to the target position
	private IEnumerator ShootProjectile(Vector3 targetPosition)
	{
		GameObject sphere = Instantiate(projectilePrefab, projectileOrigin.position, Quaternion.identity);

		while (sphere != null && Vector3.Distance(sphere.transform.position, targetPosition) > 0.01f)
		{
			// Move the sphere towards the target position
			sphere.transform.position = Vector3.MoveTowards(sphere.transform.position, targetPosition, projectileMoveSpeed * Time.deltaTime);

			yield return null;
		}
		Destroy(sphere);
	}
}
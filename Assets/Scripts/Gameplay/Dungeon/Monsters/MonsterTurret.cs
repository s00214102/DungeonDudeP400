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
	public float rotationSpeed = 5f; // how fast the turret turns to face the target

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
	private void Update() {
		// if we have a target, turn to face them
		if(detection.closestTarget!=null)
			RotateToFaceTargetOverTime();
	}
	private void AttackTarget()
	{
		if (detection.closestTarget == null || health.isDead)
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

				targetHealth.TakeDamage(attackDamage);
				OnFinishedAttacking?.Invoke();
			}
		}
	}
	private void RotateToFaceTargetOverTime(){
			Vector3 direction = detection.closestTarget.transform.position - transform.position;
			direction.y = 0;
			Quaternion targetRotation = Quaternion.LookRotation(direction);
        	transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
	}
	// Coroutine to move the sphere from its current position to the target position
	private IEnumerator ShootProjectile(Vector3 targetPosition)
	{
		GameObject sphere = Instantiate(projectilePrefab, projectileOrigin.position, Quaternion.identity);

		while (sphere != null && Vector3.Distance(sphere.transform.position, targetPosition) > 0.5f)
		{
			// Move the sphere towards the target position
			sphere.transform.position = Vector3.MoveTowards(sphere.transform.position, targetPosition, projectileMoveSpeed * Time.deltaTime);

			yield return null;
		}
		Destroy(sphere);
	}
}
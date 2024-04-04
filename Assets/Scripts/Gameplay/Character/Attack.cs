using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EntityProximityDetection))]
[RequireComponent(typeof(MonsterData))]
[RequireComponent(typeof(Health))]
public class Attack : MonoBehaviour
{
	private Health health;
	private EntityProximityDetection detection;
	internal Health targetHealth;
	private MonsterData data;
	public GameObject projectilePrefab;
	private float projectileMoveSpeed = 15f;
	public UnityEvent OnAttack;
	public UnityEvent OnFinishedAttacking;
	private void Awake()
	{
		health = GetComponent<Health>();
		detection = GetComponent<EntityProximityDetection>();
		data = GetComponent<MonsterData>();
	}
	public void StartAttacking()
	{
		if (detection.closestTarget.TryGetComponent(out targetHealth))
		{
			targetHealth.OnDied.AddListener(StopAttacking); // if the target entity dies, stop attacking
			health.OnDied.AddListener(StopAttacking); // if this entity dies, stop attacking
			InvokeRepeating("AttackTarget", 0, data.AttackRate);
		}
		else
		{
			OnFinishedAttacking?.Invoke();
		}
	}
	public void StopAttacking()
	{
		CancelInvoke("AttackTarget");
		targetHealth.OnDied.RemoveListener(StopAttacking);
		health.OnDied.RemoveListener(StopAttacking);
		OnFinishedAttacking?.Invoke();
	}
	private void AttackTarget()
	{
		// only attack if there is a target
		if (targetHealth != null)
		{
			GameObject sphere = Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
				sphere.GetComponent<Projectile>().targetPosition = detection.closestTarget.transform.position;

			targetHealth.TakeDamage(this.gameObject,data.Damage);
			OnAttack?.Invoke();
		}
		else
		{
			StopAttacking();
		}
	}

}
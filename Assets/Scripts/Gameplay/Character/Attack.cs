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
			targetHealth.EntityDied.AddListener(StopAttacking); // if the target entity dies, stop attacking
			health.EntityDied.AddListener(StopAttacking); // if this entity dies, stop attacking
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
		targetHealth.EntityDied.RemoveListener(StopAttacking);
		health.EntityDied.RemoveListener(StopAttacking);
		OnFinishedAttacking?.Invoke();
	}
	private void AttackTarget()
	{
		// only attack if there is a target
		if (targetHealth != null)
		{
			targetHealth.TakeDamage(data.Damage);
			OnAttack?.Invoke();
		}
		else
		{
			StopAttacking();
		}
	}

}
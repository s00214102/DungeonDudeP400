using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Action_DestroyCrystal : Action_Base
{
	List<System.Type> SupportedGoals = new List<System.Type> { typeof(Goal_DestroyCrystal) };
	Health crystalHealth = null;
	public GameObject projectilePrefab;
	private GameObject goal;
	private float projectileMoveSpeed = 15f;
	public UnityEvent AttackFinished;
	private void OnAttackFinished() { AttackFinished?.Invoke(); }

	public override List<System.Type> GetSupportedGoals()
	{
		return SupportedGoals;
	}
	public override float GetCost()
	{
		return 0f;
	}
	public override void OnActivated(Goal_Base _linkedGoal)
	{
		goap_debug.ChangeActionImage(5);
		base.OnActivated(_linkedGoal);

		goal = knowledge.Goal;

		InvokeRepeating("AttackTarget", 0, data.HeroData.AttackRate);
	}
	public override void OnDeactived()
	{
		base.OnDeactived();
		CancelInvoke("AttackTarget");
		crystalHealth = null;
	}
    public override void OnTick()
    {
        // face target
		if (goal!=null)
			this.gameObject.transform.rotation = Helper.RotateToFaceTargetOverTime(this.gameObject.transform, goal.transform, 5f);
    }
    private void AttackTarget()
	{
		if (crystalHealth == null)
		{
			goal = knowledge.Goal;
			crystalHealth = goal.GetComponent<Health>();
		}
		if (crystalHealth != null && !crystalHealth.isDead)
		{
			GameObject sphere = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
			sphere.GetComponent<Projectile>().targetPosition = crystalHealth.transform.position;

			crystalHealth.TakeDamage(data.HeroData.Damage);
			OnAttackFinished();
		}
	}
}
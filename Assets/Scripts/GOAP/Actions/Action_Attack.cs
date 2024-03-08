using System.Collections.Generic;
using UnityEngine;

public class Action_Attack : Action_Base
{
	List<System.Type> SupportedGoals = new List<System.Type> { typeof(Goal_Attack) };
	Health enemyHealth = null;

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
		// cache the targets Health component
		if (detection.closestTarget != null)
			enemyHealth = detection.closestTarget.GetComponent<Health>();
		InvokeRepeating("AttackTarget", 0, data.HeroData.AttackRate);
	}
	public override void OnDeactived()
	{
		base.OnDeactived();
		CancelInvoke("AttackTarget");
	}
	private void AttackTarget()
	{
		if (enemyHealth != null && !enemyHealth.isDead)
			enemyHealth.TakeDamage(data.HeroData.Damage);
	}
}
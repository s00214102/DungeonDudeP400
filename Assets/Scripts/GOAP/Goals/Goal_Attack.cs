using UnityEngine;

public class Goal_Attack : Goal_Base
{
	private int AttackPriority = 80;
	private int currentPriority = 0;

	public override int OnCalculatePriority()
	{
		return currentPriority;
	}

	public override void OnTickGoal()
	{
		currentPriority = 0;
		// if we have a target and are close enough
		if (knowledge.closestTarget != null && IsEnemyInAttackRange())
			currentPriority = AttackPriority;
	}
	public override bool CanRun()
	{
		if (knowledge.closestTarget == null)
			return false;
		return true;
	}
	private bool IsEnemyInAttackRange()
	{
		float dist = Vector3.Distance(transform.position, knowledge.closestTarget.transform.position);
		return dist <= data.HeroData.AttackRange;
	}
	public override string ToString()
	{
		return $"Attack priority: {currentPriority}";
	}
}
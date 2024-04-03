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
		var result = knowledge.RecallHighestAlertEnemy();
        if (result.found && IsEnemyInAttackRange())
			currentPriority = AttackPriority;
	}
	public override bool CanRun()
	{
		var result = knowledge.RecallHighestAlertEnemy();
		if (!result.found)
			return false;
		return true;
	}
	private bool IsEnemyInAttackRange()
	{
		var result = knowledge.RecallHighestAlertEnemy();
		float dist = Vector3.Distance(transform.position, result.enemy.transform.position);
		return dist <= data.HeroData.AttackRange;
	}
	public override string ToString()
	{
		return $"Attack priority: {currentPriority}";
	}
}
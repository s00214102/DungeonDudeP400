using UnityEngine;

public class Goal_DestroyCrystal : Goal_Base
{
	private int MaxPriority = 999;
	private int currentPriority = 0;

	public override int OnCalculatePriority()
	{
		return currentPriority;
	}
	public override void OnTickGoal()
	{
		if (knowledge.Goal == null)
			return;
		// are we at the goal? no? full priority, yes? 0 priority
		if (InGoalRange())
			currentPriority = MaxPriority;
		else
			currentPriority = 0;
	}
	public override bool CanRun()
	{
		if (knowledge.Goal == null)
			return false;
		return true;
	}
	private bool InGoalRange()
	{
		float dist = Vector3.Distance(transform.position, knowledge.Goal.transform.position);
		return dist <= data.HeroData.AttackRange;
	}
	public override string ToString()
	{
		return $"Goto goal priority: {currentPriority}";
	}
}
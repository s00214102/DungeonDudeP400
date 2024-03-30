using UnityEngine;

public class Goal_GoToGoal : Goal_Base
{
	private int GotoGoalPriority = 40;
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
		if (!InGoalRange())
			currentPriority = GotoGoalPriority;
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
		return dist <= 2;
	}
	public override string ToString()
	{
		return $"Goto goal priority: {currentPriority}";
	}
}
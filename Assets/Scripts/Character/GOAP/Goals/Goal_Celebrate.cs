using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Celebrate : Goal_Base
{
	[SerializeField] private int CelebratePriority = 100; // Define the priority of this goal
	private int currentPriority = 0;

	public override int OnCalculatePriority()
	{
		return currentPriority;
	}

	public override bool CanRun()
	{
		if (data.goal == null)
			return false;
		return true;
	}

	public override void OnTickGoal()
	{
		if (InGoalRange())
			currentPriority = CelebratePriority;
		else
			currentPriority = 0;
	}
	private bool InGoalRange()
	{
		float dist = Vector3.Distance(transform.position, data.goal);
		return dist <= 2;
	}
	public override string ToString()
	{
		return $"Goal_Celebrate: {currentPriority}";
	}
}
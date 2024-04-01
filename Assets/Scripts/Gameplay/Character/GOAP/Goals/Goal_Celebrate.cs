using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The heroes will celebrate when they destroy the crystal
public class Goal_Celebrate : Goal_Base
{
	[SerializeField] private int CelebratePriority = 999; // Define the priority of this goal
	private int currentPriority = 0;

	public override int OnCalculatePriority()
	{
		return currentPriority;
	}

	public override bool CanRun()
	{
		return true;
	}

	public override void OnTickGoal()
	{
		currentPriority = 0;
	}

	public override string ToString()
	{
		return $"Goal_Celebrate: {currentPriority}";
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// if the hero has knowledge of treasure then they can loot
// looting priority is tied to a heroes greed (personality trait)
public class Goal_Loot : Goal_Base
{
	public int Priority = 0; // Define the priority of this goal

	public override int OnCalculatePriority()
	{
		return Priority;
	}

	public override bool CanRun()
	{
		return true; // Set this to true or false based on the condition
	}

	public override void OnTickGoal()
	{
		var result = knowledge.RecallPositionByName("Treasure");
		if (!result.found)
		{
			Priority = 0;
			return;
		}
	}

	public override void OnGoalActivated(Action_Base _LinkedAction)
	{
		base.OnGoalActivated(_LinkedAction);
		// Additional activation logic here
	}

	public override void OnGoalDeactivated()
	{
		base.OnGoalDeactivated();
		// Additional deactivation logic here
	}

	public override string ToString()
	{
		return $"Goal_Loot: {Priority}";
	}
}
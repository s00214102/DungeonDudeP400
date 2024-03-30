using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// if the hero has knowledge of treasure then they can loot
// looting priority is tied to a heroes greed (personality trait)
public class Goal_Flee : Goal_Base
{
	public int Priority = 0; // Define the priority of this goal
	[SerializeField] private int maxPriority = 120;

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
		if (knowledge.Entrance == null)
		{
			Priority = 0;
			return;
		}

		// calculate flee priority based on the heroes current level of fear
		// express the heroes fear as a percentage of maxPriority
		// the fear value can be between 0-100
		int basePriority = Mathf.RoundToInt(maxPriority * (status.Fear * 0.01f));
		// reduce it somewhat by bravery
		switch (data.HeroTraits.Bravery)
		{
			case 0:
				Priority = basePriority; // no change
				break;
			case 1:
				Priority = Mathf.RoundToInt(basePriority / 1.2f); // small change, 120/1.2=100
				break;
			case 2:
				Priority = Mathf.RoundToInt(basePriority / 1.5f); // medium change, 120/1.5=80
				break;
			case 3:
				Priority = Mathf.RoundToInt(basePriority / 2); // priority cut in half, 120/2=60
				break;
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
		return $"Goal_Flee: {Priority}";
	}
}
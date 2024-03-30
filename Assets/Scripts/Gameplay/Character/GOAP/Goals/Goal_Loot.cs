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
		// try to find lootable treasure
		var result = knowledge.RecallFirstUsableItem("Treasure");
		// if a useable treasure item cant be found, set priority to 0
		if (!result.found)
		{
			Priority = 0;
			return;
		}
		else
		{
			// if lootable treasure is known, calculate priority based on greed
			switch (data.HeroTraits.Greed)
			{
				case 0:
					Priority = 10;
					break;
				case 1:
					Priority = 30;
					break;
				case 2:
					Priority = 70;
					break;
				case 3:
					Priority = 999;
					break;
				default:
					Priority = 0;
					break;
			}
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
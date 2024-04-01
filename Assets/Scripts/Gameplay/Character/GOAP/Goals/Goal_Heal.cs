using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Heal : Goal_Base
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
		//TODO if the hero has a healing potion they will use that

		// if the hero doesnt know of any healing, priority = 0
		var result = knowledge.RecallFirstItemPosition("Angel");
		if (!result.found)
		{
			Priority = 0;
			return;
		}

		int traitPriorityBonus = 0; // a value added onto the final priority as a bonus based on hero caution
		switch (data.HeroTraits.Caution)
		{
			case 0:
				traitPriorityBonus = 0;
				break;
			case 1:
				traitPriorityBonus = 10;
				break;
			case 2:
				traitPriorityBonus = 20;
				break;
			case 3:
				traitPriorityBonus = 30;
				break;
			default:
				Priority = 0;
				break;
		}

		// healing gets a higher priority as the characters health drops
		float percentMissing = (((float)health.MaxHealth - (float)health.CurrentHealth) / (float)health.MaxHealth) * 100;

		//Debug.Log(percentMissing);
		Priority = Mathf.FloorToInt(percentMissing);
		Priority += traitPriorityBonus;
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
		return $"Goal_Heal: {Priority}";
	}
}
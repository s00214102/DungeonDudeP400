using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Heal : Goal_Base
{
	[SerializeField] private int Priority = 0; // Define the priority of this goal

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
		// healing gets a higher priority as the characters health drops

		float percentMissing = (((float)health.MaxHealth - (float)health.CurrentHealth) / (float)health.MaxHealth) * 100;
		//Debug.Log(percentMissing);
		Priority = Mathf.FloorToInt(percentMissing);
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
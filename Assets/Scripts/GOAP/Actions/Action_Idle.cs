using System.Collections.Generic;
using UnityEngine;

public class Action_Idle : Action_Base
{
	List<System.Type> SupportedGoals = new List<System.Type> { typeof(Goal_Idle) };
	public override List<System.Type> GetSupportedGoals()
	{
		return SupportedGoals;
	}
	public override float GetCost()
	{
		return 0f;
	}
	public override void OnActivated(Goal_Base _linkedGoal)
	{
		base.OnActivated(_linkedGoal);
		goap_debug.ChangeActionImage(0);
	}
}
using System.Collections.Generic;
using UnityEngine;

public class Action_Celebrate : Action_Base
{
	List<System.Type> SupportedGoals = new List<System.Type> { typeof(Goal_Celebrate) };
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
		// Additional activation code here
		goap_debug.ChangeActionImage(6);
		//TODO make em dance
	}

	public override void OnDeactived()
	{
		base.OnDeactived();
		// Additional deactivation code here
	}

	public override void OnTick()
	{
		base.OnTick();
		// Additional tick code here
	}
}
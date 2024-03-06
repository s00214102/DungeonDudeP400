using UnityEngine;

public class Action_Idle : Action_Base
{
	System.Type[] SupportedGoals = new System.Type[] { typeof(Goal_Idle) };
	public override System.Type[] GetSupportedGoals()
	{
		return SupportedGoals;
	}
	public override float GetCost()
	{
		return 0f;
	}
}
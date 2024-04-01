using System.Collections.Generic;
using UnityEngine;

public class Action_Navigate : Action_Base
{
	List<System.Type> SupportedGoals = new List<System.Type> { typeof(Goal_Navigate) };
	public override List<System.Type> GetSupportedGoals()
	{
		return SupportedGoals;
	}

	//Goal_GoToGoal GotoGoal;
	public override float GetCost()
	{
		return 0f;
	}

	public override void OnActivated(Goal_Base _linkedGoal)
	{
		//GotoGoal = (Goal_GoToGoal)_linkedGoal;
		goap_debug.ChangeActionImage(2);
		base.OnActivated(_linkedGoal);
		// move to the goal
		movement.MoveTo(knowledge.Goal.transform.position);
	}

	public override void OnDeactived()
	{
		base.OnDeactived();
		// stop moving, remove listeners
		movement.StopMoving();
	}

	public override void OnTick()
	{
		base.OnTick();
		// Additional tick code here
	}
}
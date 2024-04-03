using System;
using System.Collections.Generic;
using UnityEngine;

public class Action_Engage : Action_Base
{
	List<System.Type> SupportedGoals = new List<System.Type> { typeof(Goal_Engage) };

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
		goap_debug.ChangeActionImage(4);
		base.OnActivated(_linkedGoal);

		var result = knowledge.RecallHighestAlertEnemy();
		if(result.found)
			movement.MoveTo(result.enemy.transform.position);
	}

	public override void OnDeactived()
	{
		base.OnDeactived();
		movement.StopMoving();
		//movement.DestinationReached.RemoveAllListeners();
	}
}
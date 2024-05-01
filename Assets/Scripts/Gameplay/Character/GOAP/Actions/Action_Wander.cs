using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// pick a location, go there, once there, pick a new location
public class Action_Wander : Action_Base
{
	[SerializeField] int WanderRange = 5;
	List<System.Type> SupportedGoals = new List<System.Type> { typeof(Goal_Wander) };
	public override List<System.Type> GetSupportedGoals()
	{
		return SupportedGoals;
	}
	public override float GetCost()
	{
		return 1f;
	}
	public override void OnActivated(Goal_Base _linkedGoal)
	{
		goap_debug.ChangeActionImage(1);
		base.OnActivated(_linkedGoal);
		movement.DestinationReached.AddListener(ReachedDestination);
		Wander();
	}

	private void ReachedDestination()
	{
		//movement.StopMoving();
		Wander();
	}
	private void Wander()
	{
		movement.MoveToRandomLocation(WanderRange);
	}
	public override void OnDeactived()
	{
		base.OnDeactived();
		movement.StopMoving();
		movement.DestinationReached.RemoveAllListeners();
	}
}
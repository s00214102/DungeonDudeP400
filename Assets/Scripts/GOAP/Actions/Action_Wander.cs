using System;
using UnityEngine;
using UnityEngine.Events;

// pick a location, go there, once there, pick a new location
public class Action_Wander : Action_Base
{
	System.Type[] SupportedGoals = new System.Type[] { typeof(Goal_Wander) };
	public override System.Type[] GetSupportedGoals()
	{
		return SupportedGoals;
	}
	public override float GetCost()
	{
		return 0f;
	}
	public virtual void OnActivated()
	{
		// pick a location
		movement.DestinationReached.AddListener(ReachedDestination);
	}

	private void ReachedDestination()
	{
		throw new NotImplementedException();
	}

	public virtual void OnDeactived()
	{

	}
	public virtual void OnTick()
	{
		// are we there yet? y?- OnActivated()
	}
}
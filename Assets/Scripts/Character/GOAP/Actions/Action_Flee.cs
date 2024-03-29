using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Action_Flee : Action_Base
{
	List<System.Type> SupportedGoals = new List<System.Type> { typeof(Goal_Flee) };
	public override List<System.Type> GetSupportedGoals()
	{
		return SupportedGoals;
	}

	public UnityEvent DungeonExited;
	protected virtual void OnDungeonExited() { DungeonExited?.Invoke(); }

	public override float GetCost()
	{
		return 0f;
	}

	public override void OnActivated(Goal_Base _linkedGoal)
	{
		base.OnActivated(_linkedGoal);
		// Additional activation code here
		//TODO change to flee image
		goap_debug.ChangeActionImage(2);
		movement.MoveTo(knowledge.Entrance.transform.position);
		movement.DestinationReached.AddListener(LeaveDungeon);
	}
	private void LeaveDungeon()
	{
		// when the hero reaches the entrance they leave the dungeon
		movement.StopMoving();
		OnDungeonExited();
	}
	public override void OnDeactived()
	{
		base.OnDeactived();
		// Additional deactivation code here
		movement.StopMoving();
		movement.DestinationReached.RemoveListener(LeaveDungeon);
	}

	public override void OnTick()
	{
		base.OnTick();
		// Additional tick code here
	}
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Action_Loot : Action_Base
{
	GameObject LootObject;
	List<System.Type> SupportedGoals = new List<System.Type> { typeof(Goal_Loot) };
	
	public UnityEvent Looted;
	private void OnLooted() { Looted?.Invoke(); }
	
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
		goap_debug.ChangeActionImage(2);
		// try to get healing position
		var result = knowledge.RecallPositionByName("Treasure");
		if (result.found)
		{
			LootObject = knowledge.RecallObjectByName("Treasure");
			movement.MoveTo(result.position);
			movement.DestinationReached.AddListener(Loot);
		}
	}
	private void Loot()
	{
		// when the hero reaches the treasure they will try to find loot
		goap_debug.ChangeActionImage(6);
		movement.StopMoving();
		isLooting = true;
	}
	public override void OnDeactived()
	{
		base.OnDeactived();
		// Additional deactivation code here
		movement.StopMoving();
		movement.DestinationReached.RemoveListener(Loot);
		isLooting = false;
		lootTimer = 0;
	}

private bool isLooting = false;
	private float lootTimer = 0;
	private float lootTimeToCount = 2;
	public override void OnTick()
	{
		if (LootObject == null)
			return;

		if (isLooting)
		{
			lootTimer += (1.0f * Time.deltaTime);
			if (lootTimer >= lootTimeToCount)
			{
				// if the treasure has loot, loot it
				// otherwise hero is angry/disapointed and they forget the loot
				// how to stop them detecting it again?
				Treasure treasure = LootObject.GetComponent<Treasure>();
				if (treasure.HasLoot())
				{
					inventory.AddItem(treasure.LootItem());
					OnLooted();
				}
				isLooting = false;
			}
		}
	}
}
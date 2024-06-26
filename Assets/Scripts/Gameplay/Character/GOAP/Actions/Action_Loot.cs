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

		MoveToLoot();
	}
	private void MoveToLoot()
	{
		goap_debug.ChangeActionImage(2);

		var result = knowledge.RecallFirstUsableItem("Treasure");
		if (result.found)
		{
			LootObject = result.item.GameObject;
			movement.MoveTo(result.item.LastKnownLocation);
			movement.DestinationReached.AddListener(Loot);
		}
	}
	private void Loot()
	{
		movement.DestinationReached.RemoveListener(Loot);
		// when the hero reaches the treasure they will try to find loot
		goap_debug.ChangeActionImage(3);
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

	public bool isLooting = false;
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
				Treasure treasure = LootObject.GetComponent<Treasure>();
				if (treasure.HasLoot())
				{
					//TODO image of the looted item floats above their head
					inventory.AddItem(treasure.LootItem());
					goap_debug.ChangeActionImage(6);

					// after looting the last item, the hero will remember there are no more (item no longer useable)
					if (!treasure.HasLoot())
						knowledge.ForgetItem(treasure.gameObject);
				}
				else
				{
					//TODO hero is angry/disapointed that there was no loot
					knowledge.ForgetItem(treasure.gameObject);
				}
				isLooting = false;
				lootTimer = 0;
				MoveToLoot();
				OnLooted();
			}
		}
	}
}
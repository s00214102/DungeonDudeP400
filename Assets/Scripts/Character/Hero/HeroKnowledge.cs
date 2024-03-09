using System.Collections.Generic;
using UnityEngine;

public class HeroKnowledge : MonoBehaviour
{
	public List<Entity> Entities = new List<Entity>();
	public Entity closestTarget;

	public List<TreasureMemory> Treasures = new List<TreasureMemory>();

	public void RememberTreasure(Treasure treasure, Vector3 location)
	{
		// Check if this treasure is already remembered
		var existingMemory = Treasures.Find(memory => memory.Treasure == treasure);
		if (existingMemory.Treasure != null)
		{
			// Update existing memory's location
			existingMemory.LastKnownLocation = location;
		}
		else
		{
			Treasures.Add(new TreasureMemory(treasure, location));
		}
	}
}
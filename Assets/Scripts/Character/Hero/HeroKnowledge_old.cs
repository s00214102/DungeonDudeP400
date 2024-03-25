// using System.Collections.Generic;
// using UnityEngine;

// public class HeroKnowledge_old : MonoBehaviour
// {
// 	public List<Entity> Entities = new List<Entity>();
// 	public Entity closestTarget;

// 	public List<TreasureMemory> Treasures = new List<TreasureMemory>();

// 	public List<HeroMemoryBase> memoryList = new List<HeroMemoryBase>();

// 	// make this generic, Remember(Memory memory)
// 	// create a base Memory class, holds a gameobject and a location
// 	public void RememberTreasure(Treasure treasure, Vector3 location)
// 	{
// 		// Check if this treasure is already remembered
// 		var existingMemory = Treasures.Find(memory => memory.Treasure == treasure);
// 		if (existingMemory.Treasure != null)
// 		{
// 			// Update existing memory's location
// 			existingMemory.LastKnownLocation = location;
// 		}
// 		else
// 		{
// 			Treasures.Add(new TreasureMemory(treasure, location));
// 		}
// 	}
// 	// generic version
// 	public void Remember<T>(HeroMemory<T> memory) where T : MonoBehaviour
// 	{
// 		// Check if this memory is already remembered
// 		var existingMemory = memoryList.Find(m => m.Object == memory.Object);
// 		if (existingMemory != null)
// 		{
// 			// Update existing memory's location
// 			existingMemory.LastKnownLocation = memory.LastKnownLocation;
// 		}
// 		else
// 		{
// 			memoryList.Add(memory);
// 		}
// 	}
// }
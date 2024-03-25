using System.Collections.Generic;
using UnityEngine;

public class HeroKnowledge : MonoBehaviour
{
	public List<Entity> Entities = new List<Entity>();
	public Entity closestTarget;

	public List<ItemMemory> ItemMemories = new List<ItemMemory>();

	public void Remember(string objectType, GameObject gameObject, Vector3 location, bool itemIsUsable)
	{
		// Check if this memory is already remembered
		var existingMemory = ItemMemories.Find(memory => memory.GameObject == gameObject);
		if (existingMemory.GameObject != null)
		{
			// Update existing memory's location
			existingMemory.LastKnownLocation = location;
		}
		else
		{
			ItemMemories.Add(new ItemMemory(objectType, gameObject, location, itemIsUsable));
		}
	}
	public (bool found, Vector3 position) RecallPositionByName(string objectType)
	{
		var existingMemory = ItemMemories.Find(memory => memory.ObjectType == objectType);
		if (existingMemory.GameObject != null)
		{
			return (true, existingMemory.LastKnownLocation);
		}
		else
			return (false, Vector3.zero);
	}
	public GameObject RecallObjectByName(string objectType)
	{
		var existingMemory = ItemMemories.Find(memory => memory.ObjectType == objectType);
		if (existingMemory.GameObject != null)
		{
			return existingMemory.GameObject;
		}
		else
			return null;
	}
}
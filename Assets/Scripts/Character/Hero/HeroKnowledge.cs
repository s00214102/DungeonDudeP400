using System.Collections.Generic;
using UnityEngine;

public class HeroKnowledge : MonoBehaviour
{
	public List<Entity> Entities = new List<Entity>();
	public Entity closestTarget;

	public List<HeroMemory> Memories = new List<HeroMemory>();

	public void Remember(string objectType, GameObject gameObject, Vector3 location)
	{
		// Check if this memory is already remembered
		var existingMemory = Memories.Find(memory => memory.GameObject == gameObject);
		if (existingMemory.GameObject != null)
		{
			// Update existing memory's location
			existingMemory.LastKnownLocation = location;
		}
		else
		{
			Memories.Add(new HeroMemory(objectType, gameObject, location));
		}
	}
	public (bool found, Vector3 position) RecallPositionByName(string objectType)
	{
		var existingMemory = Memories.Find(memory => memory.ObjectType == objectType);
		if (existingMemory.GameObject != null)
		{
			return (true, existingMemory.LastKnownLocation);
		}
		else
			return (false, Vector3.zero);
	}
	public GameObject RecallObjectByName(string objectType)
	{
		var existingMemory = Memories.Find(memory => memory.ObjectType == objectType);
		if (existingMemory.GameObject != null)
		{
			return existingMemory.GameObject;
		}
		else
			return null;
	}
}
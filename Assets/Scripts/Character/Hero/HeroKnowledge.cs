using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroKnowledge : MonoBehaviour
{
	//TODO make a function for adding entities
	public List<Entity> Entities = new List<Entity>();
	public Entity closestTarget;
	public List<ItemMemory> ItemMemories = new List<ItemMemory>();

	public void RememberItem(string objectType, GameObject gameObject, bool itemIsUsable)
	{
		// Check if this memory is already remembered
		var existingMemory = ItemMemories.FirstOrDefault(memory => memory.GameObject == gameObject);
		if (existingMemory != null)
		{
			// Update existing memory location if it was redetected by the detection system
			existingMemory.LastKnownLocation = gameObject.transform.position;
		}
		else
		{
			ItemMemories.Add(new ItemMemory(objectType, gameObject, gameObject.transform.position, itemIsUsable));
		}
	}
	public void ForgetItem(GameObject item)
	{
		// when a hero tries to use an item and finds it no longer useable, they will update their memory of it
		// this will stop them considering it a viable option in their goals prioritisation
		var existingMemory = ItemMemories.FirstOrDefault(memory => memory.GameObject == item);
		if (existingMemory != null)
			existingMemory.ItemIsUsable = false;
	}
	public (bool found, Vector3 position) RecallFirstItemPosition(string objectType)
	{
		var existingMemory = ItemMemories.FirstOrDefault(memory => memory.ObjectType == objectType);
		if (existingMemory != null)
		{
			return (true, existingMemory.LastKnownLocation);
		}
		else
			return (false, Vector3.zero);
	}
	public (bool found, GameObject item) RecallClosestItem(string objectType)
	{
		var existingMemories = ItemMemories.FindAll(memory => memory.ObjectType == objectType);
		if (existingMemories != null)
		{
			// if there are no items
			if (existingMemories.Count == 0)
				return (false, null);

			// if there is only one item, return it
			if (existingMemories.Count == 1)
				return (true, existingMemories[0].GameObject);

			// if there are multiple items, return the closest
			float minDistance = Mathf.Infinity;
			GameObject closestItem = null;
			foreach (var item in existingMemories)
			{
				float dist = Vector3.Distance(item.GameObject.transform.position, transform.position);
				if (dist < minDistance)
				{
					minDistance = dist;
					closestItem = item.GameObject;
				}
			}
			if (closestItem != null)
				return (true, closestItem);
		}
		return (false, null);
	}
	public (bool found, ItemMemory item) RecallFirstUsableItem(string objectType)
	{
		var existingMemory = ItemMemories.FirstOrDefault(memory => memory.ObjectType == objectType && memory.ItemIsUsable == true);
		if (existingMemory != null)
		{
			return (true, existingMemory);
		}
		else
			return (false, default(ItemMemory));
	}
	public (bool found, ItemMemory item) RecallClosestUsableItem(string objectType)
	{
		var existingMemories = ItemMemories.FindAll(memory => memory.ObjectType == objectType && memory.ItemIsUsable == true);
		if (existingMemories != null)
		{
			// if there are no items
			if (existingMemories.Count == 0)
				return (false, null);

			// if there is only one item, return it
			if (existingMemories.Count == 1)
				return (true, existingMemories[0]);

			// if there are multiple items, return the closest
			float minDistance = Mathf.Infinity;
			ItemMemory closestItem = null;
			foreach (var item in existingMemories)
			{
				float dist = Vector3.Distance(item.GameObject.transform.position, transform.position);
				if (dist < minDistance)
				{
					minDistance = dist;
					closestItem = item;
				}
			}
			if (closestItem != null)
				return (true, closestItem);
		}
		return (false, null);
	}
}
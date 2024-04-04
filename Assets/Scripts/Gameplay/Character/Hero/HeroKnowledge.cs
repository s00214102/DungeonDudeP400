using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(CharacterSenses))]
public class HeroKnowledge : MonoBehaviour
{
	//public List<Entity> Entities = new List<Entity>();
	//public Entity closestTarget;
	private CharacterSenses senses;
	public GameObject Entrance;
	public GameObject Goal;
	private void Awake()
	{
		senses = GetComponent<CharacterSenses>();
	}
	private void Start()
	{
		// find the dungeon exit/entrance
		Entrance = GameObject.Find("Entrance");
		Goal = GameObject.Find("Goal");

		// listen to character senses
		senses.OnSight.AddListener(ProcessSight);
		senses.OnHear.AddListener(ProcessSound);
		senses.OnFeel.AddListener(ProcessTouch);
	}
	private void ProcessSight(GameObject seenObject)
	{
		//TODO move what happens with the detected collider to its own component using an event
		// did we hit an enemy?
		if (seenObject.CompareTag("Enemy"))
		{
			// how alerted are we? (how close is the target)
			float alertLevel = CalculateAlertLevel(seenObject.transform);
			//TODO the enemy can be rememberd as their specific type by passing that as the name, but i need a consistent way to tell
			RememberEnemy("Enemy", seenObject, alertLevel);
		}

		// did we hit an object? check tag
		if (seenObject.CompareTag("Treasure"))
			RememberItem("Treasure", seenObject, true);

		if (seenObject.CompareTag("Angel"))
			RememberItem("Angel", seenObject, true);
	}
	private void ProcessSound(GameObject heardObject)
	{
		if (heardObject.CompareTag("Enemy"))
		{
			// how alerted are we? (how close is the target)
			float alertLevel = CalculateAlertLevel(heardObject.transform);
			//TODO the enemy can be rememberd as their specific type by passing that as the name, but i need a consistent way to tell
			RememberEnemy("Enemy", heardObject, alertLevel);
		}
	}
	private void ProcessTouch(GameObject feltObject, int damage)
	{
		if (feltObject.CompareTag("Enemy"))
		{
			// how alerted are we? (how close is the target)
			float alertLevel = CalculateAlertLevel(feltObject.transform, damage);

			//TODO the enemy can be rememberd as their specific type by passing that as the name, but i need a consistent way to tell
			RememberEnemy("Enemy", feltObject, alertLevel);
		}
	}

	#region Item Memory
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
	#endregion

	#region Enemy Memory
	public List<EnemyMemory> enemyMemories = new List<EnemyMemory>();
	public GameObject closestEnemy;
	public GameObject highestAlertEnemy;
	public void RememberEnemy(string enemyType, GameObject gameObject, float alertLevel)
	{
		// Check if this memory is already remembered
		var existingMemory = enemyMemories.FirstOrDefault(memory => memory.GameObject == gameObject);
		if (existingMemory != null)
		{
			// Update existing memory location if it was redetected by the detection system
			existingMemory.LastKnownLocation = gameObject.transform.position;
			existingMemory.AlertLevel = alertLevel;
		}
		else
		{
			enemyMemories.Add(new EnemyMemory(enemyType, gameObject, gameObject.transform.position, alertLevel));
			gameObject.GetComponent<Health>().OnDied.AddListener(delegate { ForgetEnemy(gameObject); });
		}
	}
	private void ForgetEnemy(GameObject enemy)
	{
		var existingMemory = enemyMemories.FirstOrDefault(memory => memory.GameObject == enemy);
		if (existingMemory != null)
			enemyMemories.Remove(existingMemory);
	}
	public (bool found, GameObject enemy) RecallClosestEnemy()
	{
		//var existingMemories = enemyMemories.FindAll(memory => memory.ObjectType == objectType);
		var existingMemories = enemyMemories;
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
			GameObject closestEnemy = null;
			foreach (var enemy in existingMemories)
			{
				float dist = Vector3.Distance(enemy.GameObject.transform.position, transform.position);
				if (dist < minDistance)
				{
					minDistance = dist;
					closestEnemy = enemy.GameObject;
				}
			}
			if (closestEnemy != null)
				return (true, closestEnemy);
		}
		return (false, null);
	}
	public (bool found, GameObject enemy) RecallHighestAlertEnemy()
	{
		//var existingMemories = enemyMemories.FindAll(memory => memory.ObjectType == objectType);
		var existingMemories = enemyMemories;
		if (existingMemories != null)
		{
			// if there are no items
			if (existingMemories.Count == 0)
				return (false, null);

			// if there is only one item, return it
			if (existingMemories.Count == 1)
				return (true, existingMemories[0].GameObject);

			// if there are multiple items, return the closest
			float highestAlert = 0;
			GameObject highestAlertEnemy = null;
			foreach (var enemy in existingMemories)
			{
				if (enemy.AlertLevel > highestAlert)
				{
					highestAlert = enemy.AlertLevel;
					highestAlertEnemy = enemy.GameObject;
				}
			}
			if (highestAlertEnemy != null)
				return (true, highestAlertEnemy);
		}
		return (false, null);
	}
	//TODO write this function to set the value using invoke repeating instead of alot of update calls
	private void SetHighestAlertEnemy()
	{

	}
	// alert level values
	[SerializeField] float maxDistance = 10f; // Maximum distance at which the alert level is 0
	[SerializeField] float minAlertLevel = 0f; // Minimum alert level
	[SerializeField] float maxAlertLevel = 100f; // Maximum alert level
	/// <summary>
	/// Calculate alert level based on distance to the target.
	/// </summary>
	/// <param name="enemy"></param>
	/// <returns></returns>
	private float CalculateAlertLevel(Transform enemy)
	{
		float distance = Vector3.Distance(this.transform.position, enemy.position);

		// Clamp the distance to be within the range [0, maxDistance]
		distance = Mathf.Clamp(distance, 0f, maxDistance);

		// Calculate the alert level using a linear function or any other desired function
		float alertLevel = Mathf.Lerp(maxAlertLevel, minAlertLevel, distance / maxDistance);

		return alertLevel;
	}
	private float CalculateAlertLevel(Transform enemy, int damage){
		float alertLevel = CalculateAlertLevel(enemy);
		// add more alert based on the damage value
		//TODO handy to know what the max damage of any monster is at this point
		float additionalAlertFromDamage = Mathf.Clamp(damage * 2f, 10f, 30f);

		return alertLevel+additionalAlertFromDamage;
	}
	#endregion
}
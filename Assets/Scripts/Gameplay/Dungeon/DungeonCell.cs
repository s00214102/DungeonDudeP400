using UnityEngine;
using System.Collections.Generic;


public class DungeonCell : MonoBehaviour
{
	public int movementCost = 1; // Movement cost of the cell
	public bool isWalkable = true; // Whether the cell is walkable
	public List<GameObject> occupyingObjects = new List<GameObject>(); // List of objects occupying the cell


	private void Awake()
	{

	}
	// Initialize cell properties
	public void InitializeCell()
	{
		// Add a box collider to detect other objects
		//collider = gameObject.AddComponent<BoxCollider>();
		//cellCollider.size = new Vector3(1f, 1f, 1f); // Adjust collider size as needed
	}
}

using System.Collections.Generic;
using UnityEngine;

public class DungeonCell
{
	public int movementCost = 1; // Movement cost of the cell
	public bool isWalkable = true; // Whether the cell is walkable
	public int xPos;
	public int zPos;
	public TextMesh textMesh;
	private List<GameObject> occupants = new();

	public DungeonCell(int x, int z)
	{
		xPos = x;
		zPos = z;
		movementCost = 0;
	}

	// Method to add an occupant to the cell
	public void AddOccupant(GameObject occupant)
	{
		if (!occupants.Contains(occupant))
		{
			occupants.Add(occupant);
			UpdateMovementCost();
		}
	}

	// Method to remove an occupant from the cell
	public void RemoveOccupant(Vector3Int position, GameObject occupant)
	{
		if (occupants.Contains(occupant))
		{
			occupants.Remove(occupant);
			UpdateMovementCost();
		}
	}

	// Method to update the movement cost based on the number of occupants
	private void UpdateMovementCost()
	{
		movementCost = occupants.Count;
	}
}
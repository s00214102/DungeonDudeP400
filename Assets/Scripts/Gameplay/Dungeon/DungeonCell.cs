using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DungeonCell
{
	private int baseCost = 0; // set a base cost for a cell
	public int movementCost = 0; // final value used as arc cost
	public bool isWalkable = true; // Whether the cell is walkable
	[SerializeField] public Vector3 worldPositionCenter;
	public int xPos;
	public int zPos;
	public List<DungeonCell> SeeableCells = new List<DungeonCell>(); // the cells which can be seen from this cell

	public DungeonCell(int x, int z, float cellSize)
	{
		xPos = x;
		zPos = z;
		worldPositionCenter = new Vector3(x + cellSize / 2, 0, z + cellSize / 2);
	}
	// Method to update the movement cost based on the number of occupants
	public void UpdateMovementCost(int count)
	{
		movementCost = count + baseCost;
	}
	public void SetIsWalkable(bool value)
	{
		isWalkable = value;
	}
}
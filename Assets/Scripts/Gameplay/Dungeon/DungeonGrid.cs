using System.Collections.Generic;
using UnityEngine;

public class DungeonGrid : MonoBehaviour
{
	public int gridSizeX = 10; // Grid size along the X axis
	public int gridSizeZ = 10; // Grid size along the Z axis
	public float cellSize = 1f; // Size of each cell
	public GameObject cellPrefab; // Prefab for the cell object
	private List<DungeonCell> dungeonCells = new();

	private void Start()
	{
		CreateGrid();
	}

	private void CreateGrid()
	{
		// Loop through each cell position
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int z = 0; z < gridSizeZ; z++)
			{
				// Calculate the position of the cell
				Vector3 cellPosition = new Vector3(x * cellSize, 0f, z * cellSize);

				// Create the cell object at the calculated position
				GameObject cellObject = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);

				// Set the name of the cell object for clarity in the hierarchy
				cellObject.name = $"Cell_{x}_{z}";

				// Set the cell component properties
				DungeonCell cellComponent = cellObject.GetComponent<DungeonCell>();
				if (cellComponent != null)
				{
					dungeonCells.Add(cellComponent);
					// Initialize cell properties (e.g., movement cost, walkable)
					cellComponent.InitializeCell();
				}
			}
		}
	}
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		foreach (var cell in dungeonCells)
		{
			Gizmos.DrawSphere(cell.transform.position, 0.05f);
			Gizmos.DrawWireCube(cell.transform.position, new Vector3(cellSize, cellSize, cellSize));
		}
	}
}
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public class DungeonNavigationSystem : MonoBehaviour
{
	//private DungeonGrid grid;
	public GameObject testCharacter;
	private Dictionary<Vector3Int, List<GameObject>> occupantsByPosition = new Dictionary<Vector3Int, List<GameObject>>(); // Dictionary to store occupants by position
	
	[SerializeField] private int width; // Grid size along the X axis
    [SerializeField] private int height; // Grid size along the Z axis
    [SerializeField] private float cellSize; // Size of each cell
    private DungeonCell[,] gridArray;
    private TextMesh[,] debugArray;
	private void Awake() {
		//grid = new DungeonGrid(100,100,1f);

		gridArray = new DungeonCell[width, height];
        debugArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                DungeonCell dungeonCell = new DungeonCell(x, y);
                gridArray[x,y]=dungeonCell;

                debugArray[x, y] = UtilsClass.CreateWorldText(dungeonCell.movementCost.ToString(),
                null, GetWorldPosition(x, y) + new Vector3(cellSize, 0, cellSize) * 0.5f,
                5, Color.white, TextAnchor.MiddleCenter);
                debugArray[x, y].transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
	}
	private void Start()
	{

	}
	private void Update() {
		// if(testCharacter!=null)
		// 	grid.TestHighLightCellContainingCharacter(testCharacter.transform.position);

		
	}
	public void SetValue(int x, int z, int value)
    {
        if (x > 0 && z > 0 && x < width && z < height)
        {
            gridArray[x, z].movementCost = value;
            debugArray[x, z].text = value.ToString();
        }
    }
    private Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize;
    }
    private void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        z = Mathf.FloorToInt(worldPosition.z / cellSize);
    }
	private void OnDrawGizmosSelected()
	{
		// Gizmos.color = Color.red;
		// foreach (var cell in dungeonCells)
		// {
		// 	Gizmos.DrawSphere(cell.transform.position, 0.05f);
		// 	Gizmos.DrawWireCube(cell.transform.position, new Vector3(cellSize, cellSize, cellSize));
		// }
	}
}
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Unity.VisualScripting.YamlDotNet.Serialization;
using UnityEngine;

public class DungeonNavigationSystem : MonoBehaviour
{
    [SerializeField] bool debugLog = false;
    [SerializeField] bool debugCost = false;
    // Dictionary to store occupants by position, NavAgents tell this component where they are 
    private Dictionary<Vector3Int, List<GameObject>> occupantsByPosition = new Dictionary<Vector3Int, List<GameObject>>();

    [Header("Grid Settings")]
    [SerializeField] private int width; // Grid size along the X axis
    [SerializeField] private int height; // Grid size along the Z axis
    [HideInInspector] public float cellSize; // Size of each cell
    [SerializeField] private bool debugWalkableCells; // toggle walkable cell debug draws
    private DungeonCell[,] gridArray;
    private TextMesh[,] debugCostArray;

    private void Awake()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        gridArray = new DungeonCell[width, height];
        debugCostArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                DungeonCell dungeonCell = new DungeonCell(x, y, cellSize);
                gridArray[x, y] = dungeonCell;

                // debug cost
                if (debugCost)
                {
                    debugCostArray[x, y] = UtilsClass.CreateWorldText(dungeonCell.movementCost.ToString(), null,
                GetWorldPosition(x, y) + new Vector3(cellSize, 0, cellSize) * 0.5f, 5, Color.white, TextAnchor.MiddleCenter);
                    debugCostArray[x, y].transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                }
            }
        }
    }

    private void Start()
    {

    }

    private void Update()
    {
        // if(testCharacter!=null)
        // 	grid.TestHighLightCellContainingCharacter(testCharacter.transform.position);


    }

    // updates a characters position in the list and return true if successful
    public (bool successful, Vector3Int position) UpdateCharacterPosition(GameObject character)
    {
        int x, z;
        GetXZ(character.transform.position, out x, out z);
        if (!IsXZInBounds(x, z))
        {
            Debug.LogWarning($"Character position ({x},{z}) not in bounds after converting world position.");
            return (false, Vector3Int.zero);
        }

        Vector3Int cellPosition = new Vector3Int(x, 0, z);
        // if the current position has not made a list of occupants yet
        if (!occupantsByPosition.ContainsKey(cellPosition))
            occupantsByPosition[cellPosition] = new List<GameObject>();

        // if the list for this position doesnt already contain this character
        if (!occupantsByPosition[cellPosition].Contains(character))
        {
            // add the character to the list of gameobjects occupying this cell
            occupantsByPosition[cellPosition].Add(character);

            // update the cells movement cost
            gridArray[x, z].UpdateMovementCost(GetMovementCost(cellPosition));

            // setting a cell to not walkable when a character occupies it, not recommended
            //gridArray[cellPosition.x, cellPosition.z].SetIsWalkable(false);

            // update debug showing the movement cost
            UpdateDebugMovementCost(x, z, gridArray[x, z].movementCost);

            return (true, new Vector3Int(x, 0, z)); ;
        }
        return (false, Vector3Int.zero); ;
    }

    // Remove character from a cell
    public void RemoveCharacterFromCellPosition(Vector3Int cellPosition, GameObject character)
    {
        // Remove the character from the list
        if (occupantsByPosition.ContainsKey(cellPosition) && occupantsByPosition[cellPosition].Contains(character))
            occupantsByPosition[cellPosition].Remove(character);

        // update the cells movement cost, the number of occupants * 2
        gridArray[cellPosition.x, cellPosition.z].UpdateMovementCost(GetMovementCost(cellPosition));
        //TODO set a cell back to walkable when the character leave it, this may cause issues, check how many characters are in a cell before setting isWalkable
        //gridArray[cellPosition.x, cellPosition.z].SetIsWalkable(true);

        // update debug showing the movement cost
        UpdateDebugMovementCost(cellPosition.x, cellPosition.z, gridArray[cellPosition.x, cellPosition.z].movementCost);

        // Check if there are no more characters in the cell
        if (occupantsByPosition[cellPosition].Count == 0)
        {
            occupantsByPosition.Remove(cellPosition);
        }
    }
    // get the movement cost 
    private int GetMovementCost(Vector3Int cellPosition)
    {
        return occupantsByPosition[cellPosition].Count + 3;
    }

    private void UpdateIsWalkable(bool value)
    {

    }

    public void UpdateDebugMovementCost(int x, int z, int value)
    {
        if (debugCost && IsXZInBounds(x, z))
            debugCostArray[x, z].text = value.ToString();
    }

    public void MarkCellAsNotWalkable(Vector3 position)
    {
        int x, z;
        GetXZ(position, out x, out z);
        if (IsXZInBounds(x, z))
        {
            gridArray[x, z].isWalkable = false;
        }

    }

    public bool IsXZInBounds(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
            return true;
        else
            return false;
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

    public List<Vector3> ConstructPath(Vector3 startPosition, Vector3 endPosition)
    {
        // convert positions to vector3int and check if they are in bounds
        int startX, startZ;
        GetXZ(startPosition, out startX, out startZ);
        if (!IsXZInBounds(startX, startZ))
        {
            Debug.LogWarning($"Couldnt create path, position ({startX},{startZ}) is out of bounds.");
            return new List<Vector3>();
        }
        Vector3Int start = new Vector3Int(startX, 0, startZ);

        int endX, endZ;
        GetXZ(endPosition, out endX, out endZ);
        if (!IsXZInBounds(endX, endZ))
        {
            Debug.LogWarning($"Couldnt create path, position ({endX},{endZ}) is out of bounds.");
            return new List<Vector3>();
        }
        Vector3Int end = new Vector3Int(endX, 0, endZ);

        // modify the path position to be in the center of each cell
        //List<Vector3> rawPath = DungeonPathfinding.BreadthFirstSearch2(gridArray, start, end);
        //List<Vector3> rawPath = DungeonPathfinding.BestCostFirstSearch(gridArray, start, end);
        List<Vector3> rawPath = DungeonPathfinding.AStarSearch(gridArray, start, end);

        List<Vector3> centerPath = new();
        foreach (Vector3 pos in rawPath)
        {
            centerPath.Add(new Vector3(pos.x + cellSize / 2, 0, pos.z + cellSize / 2));
        }
        return centerPath;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // foreach (var cell in dungeonCells)
        // {
        // 	Gizmos.DrawSphere(cell.transform.position, 0.05f);
        // 	Gizmos.DrawWireCube(cell.transform.position, new Vector3(cellSize, cellSize, cellSize));
        // }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // draw mesh to show non-walkable cells
                if (gridArray != null && debugWalkableCells)
                {
                    if (!gridArray[x, z].isWalkable)
                        Gizmos.DrawSphere(new Vector3(x + cellSize / 2, 1, z + cellSize / 2), 0.05f);
                }
                // draw grid lines
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 0.1f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 0.1f);
            }
        }
        // finish drawing grid lines
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 0.1f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 0.1f);
    }
}
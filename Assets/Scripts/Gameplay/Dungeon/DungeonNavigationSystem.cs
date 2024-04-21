using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using CodeMonkey.Utils;
using Unity.VisualScripting.YamlDotNet.Serialization;
using UnityEngine;

public class DungeonNavigationSystem : MonoBehaviour
{
    [SerializeField] bool debugLog = false;
    [SerializeField] bool debugCost = false;
    [SerializeField] bool debugGrid = false;
    // Dictionary to store occupants by position, NavAgents tell this component where they are 
    private Dictionary<Vector3Int, List<GameObject>> occupantsByPosition = new Dictionary<Vector3Int, List<GameObject>>();

    [Header("Grid Settings")]
    public int width; // Grid size along the X axis
    public int height; // Grid size along the Z axis
    [HideInInspector] public float cellSize; // Size of each cell
    [SerializeField] private bool debugWalkableCells; // toggle walkable cell debug draws
    [SerializeField] private DungeonCell[,] gridArray; // the full dungeon grid
    private TextMesh[,] debugCostArray;

    private void Awake()
    {
        CreateGrid();
    }

    private void Start()
    {
        // calculate each cells list of known cells
        //SeeableCellsTest();
        //PopulateSeeableCells();
        //StartCoroutine(PopulateSeeableCellsWithDelay(0.5f));
    }

    private void Update()
    {
        // if(testCharacter!=null)
        // 	grid.TestHighLightCellContainingCharacter(testCharacter.transform.position);


    }

    public void CreateGrid()
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
        #region Full grid bounds check
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
        #endregion

        // modify the path position to be in the center of each cell
        //List<Vector3> rawPath = DungeonPathfinding.BreadthFirstSearch2(gridArray, start, end);
        //List<Vector3> rawPath = DungeonPathfinding.BestCostFirstSearch(gridArray, start, end);
        List<Vector3> rawPath = DungeonPathfinding.AStarSearch(gridArray, start, end);

        // remake the path and center each position
        List<Vector3> centerPath = new();
        foreach (Vector3 pos in rawPath)
        {
            centerPath.Add(new Vector3(pos.x + cellSize / 2, 0, pos.z + cellSize / 2));
        }
        return centerPath;
    }

    #region Seeable Cell Calculation
    private void SeeableCellsTest()
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                Debug.DrawLine(gridArray[x, z].worldPositionCenter, gridArray[x, z].worldPositionCenter + new Vector3(0, 1, 0), Color.red, 1);
            }
    }
    public void StartSeeableCellCalculation()
    {
        StartCoroutine(PopulateSeeableCellsWithDelay());
    }
    public void StopSeeableCellCalculation()
    {
        StopCoroutine(PopulateSeeableCellsWithDelay());
    }
    private IEnumerator PopulateSeeableCellsWithDelay()
    {
        int range = 5;
        // Iterate over each cell in the grid array
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                DungeonCell sourceCell = gridArray[x, z];
                if (!sourceCell.isWalkable)
                    continue;

                sourceCell.SeeableCells = new List<DungeonCell>();

                // Cast rays from the source cell to other cells within the set range
                int xMin = Mathf.Clamp(x - range, 0, gridArray.GetLength(0));
                int xMax = Mathf.Clamp(x + range, 0, gridArray.GetLength(0));
                for (int i = xMin; i < xMax; i++)
                {
                    int zMin = Mathf.Clamp(z - range, 0, gridArray.GetLength(1));
                    int zMax = Mathf.Clamp(z + range, 0, gridArray.GetLength(1));
                    for (int j = zMin; j < zMax; j++)
                    {
                        DungeonCell targetCell = gridArray[i, j];

                        // Skip if source and target cells are the same or target cell isnt walkable
                        if (sourceCell == targetCell || !targetCell.isWalkable)
                            continue;

                        // Cast a ray from the source cell to the target cell
                        Vector3 sourcePosition = sourceCell.worldPositionCenter + new Vector3(0, cellSize / 2, 0); // raise the starting position up on the Y axis a bit
                        Vector3 targetPosition = targetCell.worldPositionCenter + new Vector3(0, cellSize / 2, 0);
                        Vector3 direction = targetPosition - sourcePosition;

                        Debug.Log($"Checking from {sourcePosition} to {targetPosition}.");

                        //yield return new WaitForSeconds(waitTime);
                        Debug.DrawLine(sourcePosition, targetPosition, Color.red, 0.5f); // Visualize the ray

                        RaycastHit hit;
                        // set the length to stop it 
                        if (Physics.Raycast(sourcePosition, direction, out hit, direction.magnitude))
                        {
                            // If the ray doesn't hit anything tagged "Block", add the target cell to the source cell's SeeableCells
                            if (hit.collider != null && hit.collider.CompareTag("Block"))
                            {
                                sourceCell.SeeableCells.Add(targetCell);
                            }
                        }
                        else
                        {
                            sourceCell.SeeableCells.Add(targetCell);
                            Debug.DrawLine(sourcePosition, targetPosition, Color.red, 0.5f); // Visualize the ray
                            Debug.Log($"{sourcePosition} can see {targetPosition}");
                        }
                        // move to next frame
                        yield return null;
                    }
                }
            }
        }
    }
    public void StopCellCalculation()
    {
        stopCalculation = true;
    }

    // currently using this one
    private bool stopCalculation = false;
    public void PopulateSeeableCellsForEditor()
    {
        int stepCounter = 0;
        stopCalculation = false;
        float startTime = Time.realtimeSinceStartup;
        int range = 5;
        // Iterate over each cell in the grid array
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            if (stopCalculation)
                break;
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                if (stopCalculation)
                    break;

                DungeonCell sourceCell = gridArray[x, z];
                if (!sourceCell.isWalkable)
                    continue;

                if (sourceCell.SeeableCells == null)
                    sourceCell.SeeableCells = new List<DungeonCell>();

                // Cast rays from the source cell to other cells within the set range
                int xMin = Mathf.Clamp(x - range, 0, gridArray.GetLength(0));
                int xMax = Mathf.Clamp(x + range, 0, gridArray.GetLength(0));
                for (int i = xMin; i < xMax; i++)
                {
                    int zMin = Mathf.Clamp(z - range, 0, gridArray.GetLength(1));
                    int zMax = Mathf.Clamp(z + range, 0, gridArray.GetLength(1));
                    for (int j = zMin; j < zMax; j++)
                    {
                        DungeonCell targetCell = gridArray[i, j];

                        if (targetCell.SeeableCells == null)
                            targetCell.SeeableCells = new List<DungeonCell>();

                        // Skip if source and target cells are the same OR target cell isnt walkable OR target cell is already in this cells list
                        if (sourceCell == targetCell || !targetCell.isWalkable || sourceCell.SeeableCells.Contains(targetCell))
                            continue;

                        // Cast a ray from the source cell to the target cell
                        Vector3 sourcePosition = sourceCell.worldPositionCenter + new Vector3(0, cellSize / 2, 0); // raise the starting position up on the Y axis a bit
                        Vector3 targetPosition = targetCell.worldPositionCenter + new Vector3(0, cellSize / 2, 0);
                        Vector3 direction = targetPosition - sourcePosition;

                        //Debug.Log($"Checking from {sourcePosition} to {targetPosition}.");

                        //yield return new WaitForSeconds(waitTime);
                        //Debug.DrawLine(sourcePosition, targetPosition, Color.red, 0.5f); // Visualize the ray

                        RaycastHit hit;
                        // set the length to stop it 
                        if (Physics.Raycast(sourcePosition, direction, out hit, direction.magnitude))
                        {
                            // If the ray doesn't hit anything tagged "Block", add the target cell to the source cell's SeeableCells
                            if (hit.collider != null && hit.collider.CompareTag("Block"))
                            {
                                //sourceCell.SeeableCells.Add(targetCell);
                            }
                        }
                        else
                        {
                            if (hit.collider == null)
                            {
                                sourceCell.SeeableCells.Add(targetCell);
                                targetCell.SeeableCells.Add(sourceCell);
                                stepCounter++;
                                Debug.Log($"{sourcePosition} can see {targetPosition}");
                            }
                        }
                    }
                }
            }
        }
        float endTime = Time.realtimeSinceStartup;
        Debug.Log($"PopulateSeeableCells method completed in {(endTime - startTime):F4} seconds.");
        Debug.Log($"Steps taken: {stepCounter}.");
    }

    private void PopulateSeeableCells()
    {
        Time.timeScale = 0.01f;
        int range = 5;
        // Iterate over each cell in the grid array
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                DungeonCell sourceCell = gridArray[x, z];
                if (!sourceCell.isWalkable)
                    continue;

                sourceCell.SeeableCells = new List<DungeonCell>();

                // Cast rays from the source cell to other cells within the set range
                int xMin = Mathf.Clamp(x - range, 0, gridArray.GetLength(0));
                int xMax = Mathf.Clamp(x + range, 0, gridArray.GetLength(0));
                for (int i = xMin; i < xMax; i++)
                {
                    int zMin = Mathf.Clamp(z - range, 0, gridArray.GetLength(1));
                    int zMax = Mathf.Clamp(z + range, 0, gridArray.GetLength(1));
                    for (int j = zMin; j < zMax; j++)
                    {
                        DungeonCell targetCell = gridArray[i, j];

                        // Skip if source and target cells are the same
                        if (sourceCell == targetCell || !targetCell.isWalkable)
                            continue;

                        // Cast a ray from the source cell to the target cell
                        Vector3 sourcePosition = sourceCell.worldPositionCenter + new Vector3(0, cellSize / 2, 0); // raise the starting position up on the Y axis a bit
                        Vector3 targetPosition = targetCell.worldPositionCenter + new Vector3(0, cellSize / 2, 0);
                        Vector3 direction = targetPosition - sourcePosition;

                        //Debug.Log($"Checking from {sourcePosition} to {targetPosition}.");
                        Debug.DrawLine(sourcePosition, targetPosition, Color.red, 0.1f); // Visualize the ray
                        RaycastHit hit;
                        if (!Physics.Raycast(sourcePosition, direction, out hit, Mathf.Infinity))
                        {
                            Debug.Log("No hit");
                            // If the ray doesn't hit anything tagged "Block", add the target cell to the source cell's SeeableCells
                            // if (hit.collider != null && hit.collider.CompareTag("Block"))
                            // {
                            //     sourceCell.SeeableCells.Add(targetCell);
                            // }
                        }
                    }
                }
            }
        }
        Time.timeScale = 1f;
    }

    public void VisualizeCellNeighbours(Vector3 cellPos)
    {
        if (gridArray == null)
        { Debug.LogWarning("Grid array is null"); return; }

        int x, z;
        GetXZ(cellPos, out x, out z);

        if (!IsXZInBounds(x, z))
        { Debug.LogWarning("Cell position not in bounds"); return; }

        DungeonCell targetCell = gridArray[x, z];

        if (targetCell.SeeableCells == null)
        { Debug.LogWarning("Cell list of neighbours is null"); return; }

        StartCoroutine(DrawLineForX(targetCell.worldPositionCenter, targetCell.worldPositionCenter + new Vector3(0, 1, 0)));
        foreach (var neighbour in targetCell.SeeableCells)
        {
            Debug.Log($"Neighbour position: {neighbour.worldPositionCenter}");
            //Debug.DrawLine(neighbour.worldPositionCenter, neighbour.worldPositionCenter + new Vector3(0, 1, 0));
            StartCoroutine(DrawLineForX(neighbour.worldPositionCenter, neighbour.worldPositionCenter + new Vector3(0, 1, 0)));
        }
    }

    private IEnumerator DrawLineForX(Vector3 pos1, Vector3 pos2)
    {
        for (int i = 0; i < 3; i++)
        {
            Debug.DrawLine(pos1, pos2);
            yield return null;
        }
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        #region Draw Grid
        if (debugGrid)
        {
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
        #endregion
    }
}
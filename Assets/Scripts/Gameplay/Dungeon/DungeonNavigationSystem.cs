using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using CodeMonkey.Utils;
using Unity.VisualScripting.YamlDotNet.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

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
    public DungeonCell[,] dungeonGrid; // the full dungeon grid
    private TextMesh[,] debugCostArray;
    [SerializeField] private GameObject levelLayout;
    [SerializeField] private DungeonCharacterManager dungeonCharacterManager;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        Debug.Log("Initializing DungeonNavigationSystem.");
        CreateGrid();
        PopulateSeeableCells();
        dungeonCharacterManager.Init(this);
    }

    public void CreateGrid()
    {
        dungeonGrid = new DungeonCell[width, height];
        debugCostArray = new TextMesh[width, height];

        List<DungeonBlock> blocks = levelLayout.GetComponentsInChildren<DungeonBlock>().ToList();

        for (int x = 0; x < dungeonGrid.GetLength(0); x++)
        {
            for (int y = 0; y < dungeonGrid.GetLength(1); y++)
            {
                DungeonCell dungeonCell = new DungeonCell(x, y, cellSize);
                dungeonGrid[x, y] = dungeonCell;

                // debug cost
                if (debugCost)
                {
                    debugCostArray[x, y] = UtilsClass.CreateWorldText(dungeonCell.movementCost.ToString(), null,
                    GetWorldPosFromCellCoords(x, y) + new Vector3(cellSize, 0, cellSize) * 0.5f, 5, Color.white, TextAnchor.MiddleCenter);
                    debugCostArray[x, y].transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                }
            }
        }
        Debug.Log($"marking cells with blocks as not walkable, {blocks.Count} blocks to process.");
        foreach (var block in blocks)
        {
            // every block finds its place/cell on the grid, and marks that cell as not walkable
            MarkCellAsNotWalkable(block.transform.position);
            // block world position to cell grid position
            int x, z;
            GetCellCoordsFromWorldPos(block.transform.position, out x, out z);
            if (IsXZInBounds(x, z))
                AddDebugSphere(notWalkableCellSpheres, dungeonGrid[x, z].worldPositionCenter, Color.magenta, 2.0f);
        }
    }

    // updates a characters position in the list and return true if successful
    public (bool successful, Vector3Int position) UpdateCharacterPosition(GameObject character)
    {
        int x, z;
        GetCellCoordsFromWorldPos(character.transform.position, out x, out z);
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
            dungeonGrid[x, z].UpdateMovementCost(GetMovementCost(cellPosition));

            // setting a cell to not walkable when a character occupies it, not recommended
            //gridArray[cellPosition.x, cellPosition.z].SetIsWalkable(false);

            // update debug showing the movement cost
            UpdateDebugMovementCost(x, z, dungeonGrid[x, z].movementCost);

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
        dungeonGrid[cellPosition.x, cellPosition.z].UpdateMovementCost(GetMovementCost(cellPosition));
        //TODO set a cell back to walkable when the character leaves it, this may cause issues, check how many characters are in a cell before setting isWalkable
        //gridArray[cellPosition.x, cellPosition.z].SetIsWalkable(true);

        // update debug showing the movement cost
        UpdateDebugMovementCost(cellPosition.x, cellPosition.z, dungeonGrid[cellPosition.x, cellPosition.z].movementCost);

        // Check if there are no more characters in the cell
        if (occupantsByPosition[cellPosition].Count == 0)
        {
            occupantsByPosition.Remove(cellPosition);
        }
    }

    // store agent paths in a dict, accessed by their id
    public Dictionary<int, List<Vector3>> agentPaths = new();
    // Create a path for an agent to move from its current position to a target position
    public void CreatePathToTargetForAgent(int agentId, Vector3 startPosition, Vector3 endPosition)
    {
        // convert positions to vector3int and check if they are in bounds
        #region Full grid bounds check
        int startX, startZ;
        GetCellCoordsFromWorldPos(startPosition, out startX, out startZ);
        if (!IsXZInBounds(startX, startZ))
        {
            Debug.LogWarning($"Couldnt create path, position ({startX},{startZ}) is out of bounds.");
            //return new List<Vector3>();
        }
        Vector3Int start = new Vector3Int(startX, 0, startZ);

        int endX, endZ;
        GetCellCoordsFromWorldPos(endPosition, out endX, out endZ);
        if (!IsXZInBounds(endX, endZ))
        {
            Debug.LogWarning($"Couldnt create path, position ({endX},{endZ}) is out of bounds.");
            //return new List<Vector3>();
        }
        Vector3Int end = new Vector3Int(endX, 0, endZ);
        #endregion

        // modify the path position to be in the center of each cell
        //List<Vector3> rawPath = DungeonPathfinding.BreadthFirstSearch2(gridArray, start, end);
        //List<Vector3> rawPath = DungeonPathfinding.BestCostFirstSearch(gridArray, start, end);
        List<Vector3> rawPath = DungeonPathfinding.AStarSearch(dungeonGrid, start, end);

        // remake the path and center each position
        List<Vector3> centerPath = new();
        foreach (Vector3 pos in rawPath)
        {
            centerPath.Add(new Vector3(pos.x + cellSize / 2, 0, pos.z + cellSize / 2));
        }
        agentPaths[agentId] = centerPath;
        //return centerPath;
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
        GetCellCoordsFromWorldPos(position, out x, out z);
        if (IsXZInBounds(x, z))
        {
            dungeonGrid[x, z].isWalkable = false;
        }
    }

    public bool IsXZInBounds(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
            return true;
        else
            return false;
    }

    private Vector3 GetWorldPosFromCellCoords(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize;
    }

    private void GetCellCoordsFromWorldPos(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        z = Mathf.FloorToInt(worldPosition.z / cellSize);
    }
    //TODO should use Vector2Int
    public Vector2 GetCellCoordsFromWorldPos(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int z = Mathf.FloorToInt(worldPosition.z / cellSize);
        return new Vector2(x, z);
    }

    /// <summary>
    /// 
    /// </summary>
    public List<DungeonCell> GetSeeableCellsFromPosition(Vector3 currentPos)
    {
        // get the current cell the agent is in from the current position
        Vector2 coords = GetCellCoordsFromWorldPos(currentPos);
        DungeonCell currentCell = dungeonGrid[(int)coords.x, (int)coords.y];
        List<DungeonCell> newCells = currentCell.SeeableCells;
        return currentCell.SeeableCells;
    }

    #region Seeable Cell Calculation
    private void SeeableCellsTest()
    {
        for (int x = 0; x < dungeonGrid.GetLength(0); x++)
            for (int z = 0; z < dungeonGrid.GetLength(1); z++)
            {
                Debug.DrawLine(dungeonGrid[x, z].worldPositionCenter, dungeonGrid[x, z].worldPositionCenter + new Vector3(0, 1, 0), Color.red, 1);
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
        for (int x = 0; x < dungeonGrid.GetLength(0); x++)
        {
            for (int z = 0; z < dungeonGrid.GetLength(1); z++)
            {
                DungeonCell sourceCell = dungeonGrid[x, z];
                if (!sourceCell.isWalkable)
                    continue;

                sourceCell.SeeableCells = new List<DungeonCell>();

                // Cast rays from the source cell to other cells within the set range
                int xMin = Mathf.Clamp(x - range, 0, dungeonGrid.GetLength(0));
                int xMax = Mathf.Clamp(x + range, 0, dungeonGrid.GetLength(0));
                for (int i = xMin; i < xMax; i++)
                {
                    int zMin = Mathf.Clamp(z - range, 0, dungeonGrid.GetLength(1));
                    int zMax = Mathf.Clamp(z + range, 0, dungeonGrid.GetLength(1));
                    for (int j = zMin; j < zMax; j++)
                    {
                        DungeonCell targetCell = dungeonGrid[i, j];

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
    public void PopulateSeeableCells()
    {
        int stepCounter = 0;
        stopCalculation = false;
        float startTime = Time.realtimeSinceStartup;
        int range = 5;
        // Iterate over each cell in the grid array
        for (int x = 0; x < dungeonGrid.GetLength(0); x++)
        {
            if (stopCalculation)
                break;
            for (int z = 0; z < dungeonGrid.GetLength(1); z++)
            {
                if (stopCalculation)
                    break;

                DungeonCell sourceCell = dungeonGrid[x, z];
                if (!sourceCell.isWalkable)
                    continue;

                if (sourceCell.SeeableCells == null)
                    sourceCell.SeeableCells = new List<DungeonCell>();

                // Cast rays from the source cell to other cells within the set range
                int xMin = Mathf.Clamp(x - range, 0, dungeonGrid.GetLength(0));
                int xMax = Mathf.Clamp(x + range, 0, dungeonGrid.GetLength(0));
                for (int i = xMin; i < xMax; i++)
                {
                    int zMin = Mathf.Clamp(z - range, 0, dungeonGrid.GetLength(1));
                    int zMax = Mathf.Clamp(z + range, 0, dungeonGrid.GetLength(1));
                    for (int j = zMin; j < zMax; j++)
                    {
                        DungeonCell targetCell = dungeonGrid[i, j];

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
                        if (!Physics.Raycast(sourcePosition, direction, out hit, direction.magnitude))
                        {
                            sourceCell.SeeableCells.Add(targetCell);
                            targetCell.SeeableCells.Add(sourceCell);
                            stepCounter++;
                            //Debug.Log($"{sourcePosition} can see {targetPosition}");
                        }


                        // if (Physics.Raycast(sourcePosition, direction, out hit, direction.magnitude))
                        // {
                        //     // If the ray doesn't hit anything tagged "Block", add the target cell to the source cell's SeeableCells        
                        //     if (hit.collider != null && hit.collider.CompareTag("Block"))
                        //     {

                        //     }

                        // }
                        // else // the ray didnt hit any collider
                        // {
                        //     if (hit.collider == null)
                        //     {
                        //         sourceCell.SeeableCells.Add(targetCell);
                        //         targetCell.SeeableCells.Add(sourceCell);
                        //         stepCounter++;
                        //         Debug.Log($"{sourcePosition} can see {targetPosition}");
                        //     }
                        // }
                    }
                }
            }
        }
        float endTime = Time.realtimeSinceStartup;
        Debug.Log($"PopulateSeeableCells method completed in {(endTime - startTime):F4} seconds.");
        Debug.Log($"Steps taken: {stepCounter}.");
    }
    private List<DebugRay> debugRays = new List<DebugRay>();
    private void AddDebugRay(Vector3 start, Vector3 end, Color color, float duration)
    {
        debugRays.Add(new DebugRay
        {
            start = start,
            end = end,
            color = color,
            expirationTime = Time.realtimeSinceStartup + duration
        });

        // Force a repaint so the rays are drawn immediately
        SceneView.RepaintAll();
    }
    private void PopulateSeeableCellsOld()
    {
        Time.timeScale = 0.01f;
        int range = 5;
        // Iterate over each cell in the grid array
        for (int x = 0; x < dungeonGrid.GetLength(0); x++)
        {
            for (int z = 0; z < dungeonGrid.GetLength(1); z++)
            {
                DungeonCell sourceCell = dungeonGrid[x, z];
                if (!sourceCell.isWalkable)
                    continue;

                sourceCell.SeeableCells = new List<DungeonCell>();

                // Cast rays from the source cell to other cells within the set range
                int xMin = Mathf.Clamp(x - range, 0, dungeonGrid.GetLength(0));
                int xMax = Mathf.Clamp(x + range, 0, dungeonGrid.GetLength(0));
                for (int i = xMin; i < xMax; i++)
                {
                    int zMin = Mathf.Clamp(z - range, 0, dungeonGrid.GetLength(1));
                    int zMax = Mathf.Clamp(z + range, 0, dungeonGrid.GetLength(1));
                    for (int j = zMin; j < zMax; j++)
                    {
                        DungeonCell targetCell = dungeonGrid[i, j];

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

#if UNITY_EDITOR
    public void VisualizeCellNeighbours(Vector3 cellPos)
    {
        if (dungeonGrid == null)
        { Debug.LogWarning("Grid array is null"); return; }

        int x, z;
        GetCellCoordsFromWorldPos(cellPos, out x, out z);

        if (!IsXZInBounds(x, z))
        { Debug.LogWarning("Cell position not in bounds"); return; }

        DungeonCell targetCell = dungeonGrid[x, z];

        if (!targetCell.isWalkable)
        { Debug.LogWarning("Cell is not walkable."); return; }

        if (targetCell.SeeableCells == null)
        { Debug.LogWarning("Cell list of neighbours is null"); return; }


        // Add the target cell for visualization
        AddDebugSphere(seeableCellSpheres, targetCell.worldPositionCenter, Color.green, 2.0f);

        // Add all neighbors for visualization
        foreach (var neighbour in targetCell.SeeableCells)
        {
            Debug.Log($"Neighbour position: {neighbour.worldPositionCenter}");
            AddDebugSphere(seeableCellSpheres, neighbour.worldPositionCenter, Color.red, 2.0f);
        }

        // StartCoroutine(DrawLineForX(targetCell.worldPositionCenter, targetCell.worldPositionCenter + new Vector3(0, 1, 0)));
        // foreach (var neighbour in targetCell.SeeableCells)
        // {
        //     Debug.Log($"Neighbour position: {neighbour.worldPositionCenter}");
        //     //Debug.DrawLine(neighbour.worldPositionCenter, neighbour.worldPositionCenter + new Vector3(0, 1, 0));
        //     StartCoroutine(DrawLineForX(neighbour.worldPositionCenter, neighbour.worldPositionCenter + new Vector3(0, 1, 0)));
        // }
    }

    private IEnumerator DrawLineForX(Vector3 pos1, Vector3 pos2)
    {
        for (int i = 0; i < 3; i++)
        {
            Debug.DrawLine(pos1, pos2, Color.green);
            yield return null;
        }
        yield return new WaitForSeconds(2);
    }

    private List<DebugSphere> seeableCellSpheres = new List<DebugSphere>();
    private List<DebugSphere> notWalkableCellSpheres = new List<DebugSphere>();
    private void AddDebugSphere(List<DebugSphere> debugSpheres, Vector3 position, Color color, float duration)
    {
        debugSpheres.Add(new DebugSphere
        {
            position = position,
            color = color,
            expirationTime = Time.realtimeSinceStartup + duration
        });

        // Force the editor to repaint so the sphere is drawn immediately
        SceneView.RepaintAll();
    }

    public void DebugSeeableCellsFromCell(Vector3 sourceCellPos)
    {
        DungeonCell debugSourceCell = dungeonGrid[(int)sourceCellPos.x, (int)sourceCellPos.z];
        int debugRange = 6;

        if (debugSourceCell == null || !debugSourceCell.isWalkable)
        {
            Debug.LogWarning("Invalid source cell for debugging.");
            return;
        }


        int x, z;
        GetCellCoordsFromWorldPos(debugSourceCell.worldPositionCenter, out x, out z);

        // Define range bounds
        int xMin = Mathf.Clamp(x - debugRange, 0, dungeonGrid.GetLength(0));
        int xMax = Mathf.Clamp(x + debugRange, 0, dungeonGrid.GetLength(0));
        int zMin = Mathf.Clamp(z - debugRange, 0, dungeonGrid.GetLength(1));
        int zMax = Mathf.Clamp(z + debugRange, 0, dungeonGrid.GetLength(1));

        for (int i = xMin; i < xMax; i++)
        {
            for (int j = zMin; j < zMax; j++)
            {
                DungeonCell targetCell = dungeonGrid[i, j];

                if (targetCell == null || !targetCell.isWalkable || debugSourceCell == targetCell)
                    continue;

                Vector3 sourcePosition = debugSourceCell.worldPositionCenter + new Vector3(0, cellSize / 2, 0); // raise the starting position up on the Y axis a bit
                Vector3 targetPosition = targetCell.worldPositionCenter + new Vector3(0, cellSize / 2, 0);
                Vector3 direction = targetPosition - sourcePosition;

                // Perform the raycast
                RaycastHit hit;
                bool isBlocked = false;
                if (!Physics.Raycast(sourcePosition, direction, out hit, direction.magnitude))
                {
                    // Visualize the ray for this specific debug scenario
                    AddDebugRay(sourcePosition, targetPosition, isBlocked ? Color.red : Color.green, 2.0f);
                }
                else
                {
                    isBlocked = true;
                    AddDebugRay(sourcePosition, targetPosition, isBlocked ? Color.red : Color.green, 2.0f);
                    // Log detailed information for debugging
                    Debug.Log($"Ray from {sourcePosition} to {targetPosition}: {(isBlocked ? "Blocked" : "Unblocked")}");
                }



                // if (Physics.Raycast(sourcePosition, direction.normalized, out hit, direction.magnitude))
                // {
                //     if (hit.collider != null && hit.collider.CompareTag("Block"))
                //     {
                //         isBlocked = true;
                //     }
                // }
                // else
                // {

                //     // Visualize the ray for this specific debug scenario
                //     AddDebugRay(sourcePosition, targetPosition, isBlocked ? Color.red : Color.green, 2.0f);

                //     // Log detailed information for debugging
                //     Debug.Log($"Ray from {sourcePosition} to {targetPosition}: {(isBlocked ? "Blocked" : "Unblocked")}");
                // }
            }
        }
    }


    private void OnDrawGizmos()
    {
        // Draw all debug spheres
        for (int i = seeableCellSpheres.Count - 1; i >= 0; i--)
        {
            var sphere = seeableCellSpheres[i];
            if (Time.realtimeSinceStartup > sphere.expirationTime)
            {
                seeableCellSpheres.RemoveAt(i);
            }
            else
            {
                Handles.color = sphere.color;
                Handles.SphereHandleCap(0, sphere.position, Quaternion.identity, 0.5f, EventType.Repaint);
            }
        }
        for (int i = notWalkableCellSpheres.Count - 1; i >= 0; i--)
        {
            var sphere = notWalkableCellSpheres[i];
            if (Time.realtimeSinceStartup > sphere.expirationTime)
            {
                notWalkableCellSpheres.RemoveAt(i);
            }
            else
            {
                Handles.color = sphere.color;
                Handles.SphereHandleCap(0, sphere.position, Quaternion.identity, 0.5f, EventType.Repaint);
            }
        }
        // Draw all debug rays
        for (int i = debugRays.Count - 1; i >= 0; i--)
        {
            var ray = debugRays[i];
            if (Time.realtimeSinceStartup > ray.expirationTime)
            {
                debugRays.RemoveAt(i);
            }
            else
            {
                Handles.color = ray.color;
                Handles.DrawLine(ray.start, ray.end);
            }
        }
    }

    private class DebugSphere
    {
        public Vector3 position;
        public Color color;
        public float expirationTime;
    }

    private class DebugRay
    {
        public Vector3 start;
        public Vector3 end;
        public Color color;
        public float expirationTime;
    }
#endif
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
                    if (dungeonGrid != null && debugWalkableCells)
                    {
                        if (!dungeonGrid[x, z].isWalkable)
                            Gizmos.DrawSphere(new Vector3(x + cellSize / 2, 1, z + cellSize / 2), 0.05f);
                    }
                    // draw grid lines
                    Debug.DrawLine(GetWorldPosFromCellCoords(x, z), GetWorldPosFromCellCoords(x, z + 1), Color.white, 0.1f);
                    Debug.DrawLine(GetWorldPosFromCellCoords(x, z), GetWorldPosFromCellCoords(x + 1, z), Color.white, 0.1f);
                }
            }
            // finish drawing grid lines
            Debug.DrawLine(GetWorldPosFromCellCoords(0, height), GetWorldPosFromCellCoords(width, height), Color.white, 0.1f);
            Debug.DrawLine(GetWorldPosFromCellCoords(width, 0), GetWorldPosFromCellCoords(width, height), Color.white, 0.1f);
        }
        #endregion
    }
}
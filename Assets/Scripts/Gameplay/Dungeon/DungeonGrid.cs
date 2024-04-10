using CodeMonkey.Utils;
using Codice.Client.GameUI.Update;
using UnityEngine;

public class DungeonGrid
{
    private int width; // Grid size along the X axis
    private int height; // Grid size along the Z axis
    private float cellSize; // Size of each cell
    private DungeonCell[,] gridArray;
    private TextMesh[,] debugArray;
    public DungeonGrid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new DungeonCell[width, height];
        debugArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {

                DungeonCell dungeonCell = new DungeonCell(x, y, cellSize);
                gridArray[x, y] = dungeonCell;

                debugArray[x, y] = UtilsClass.CreateWorldText($"({x},{y})", null,
                GetWorldPosition(x, y) + new Vector3(cellSize, 0, cellSize) * 0.5f, 5, Color.white, TextAnchor.MiddleCenter);
                debugArray[x, y].transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                //dungeonCell.textMesh = debugArray[x, y];

                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }
    public void SetValue(int x, int z, int value)
    {
        if (x > 0 && z > 0 && x < width && z < height)
        {
            //Debug.Log(gridArray[x,z].movementCost);
            gridArray[x, z].movementCost = value;
            //debugArray[x, z].text = value.ToString();
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
    public void TestHighLightCellContainingCharacter(Vector3 position)
    {
        int x, z;
        GetXZ(position, out x, out z);
        SetValue(x, z, 99);
    }
}
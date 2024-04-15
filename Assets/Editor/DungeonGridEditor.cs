#if (UNITY_EDITOR)
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonNavigationSystem))]
public class DungeonGridEditor : Editor
{
	public Vector3 cellPos;
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		DungeonNavigationSystem _navSystem = (DungeonNavigationSystem)target;

		GUILayout.Label("Select a cell on the grid and click 'view' to view its seeable neighbours.");
		EditorGUILayout.BeginHorizontal();
		cellPos = EditorGUILayout.Vector3Field("Cell", cellPos);
		if (GUILayout.Button("View"))
			_navSystem.VisualizeCellNeighbours(cellPos);
		EditorGUILayout.EndHorizontal();

		if (GUILayout.Button("Create Grid"))
		{
			_navSystem.CreateGrid();
		}
		if (GUILayout.Button("Start Cell Calculation"))
		{
			//_navSystem.StartSeeableCellCalculation();
			_navSystem.PopulateSeeableCellsForEditor();
		}
		if (GUILayout.Button("Stop Cell Calculation"))
		{
			//_navSystem.StopSeeableCellCalculation();
			_navSystem.StopCellCalculation();
		}

		EditorGUILayout.HelpBox("help.", MessageType.Info);
	}
}
#endif
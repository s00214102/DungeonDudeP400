using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Goal_Debug : MonoBehaviour
{
	//[SerializeField] private GUIStyle style;
	//[SerializeField] private Vector3 offset;
	[SerializeField] private List<Goal_Base> Goals = new();
	public Rect displayRect = new Rect(10, 10, 300, 20);

	private void Awake()
	{
		Goal_Base[] _goals = GetComponents<Goal_Base>();
		foreach (var goal in _goals)
		{
			Goals.Add(goal);
		}
	}
	private void OnGUI()
	{
		if (Selection.activeGameObject == gameObject)
		{
			// Start Y position for drawing text
			float currentY = displayRect.y;


			foreach (Component goal in Goals)
			{
				// For each component, draw its ToString() return value on the screen
				GUI.Label(new Rect(displayRect.x, currentY, displayRect.width, displayRect.height), goal.ToString());

				// Increment the Y position for the next component
				currentY += displayRect.height;
			}
		}
	}
	// private void OnDrawGizmosSelected()
	// {
	// 	string text = "Goal priorities\n";
	// 	foreach (var goal in Goals)
	// 	{
	// 		text += goal.name;
	// 		text += goal.ToString();
	// 	}
	// 	UnityEditor.Handles.Label(transform.position + offset, text, style);
	// }
}
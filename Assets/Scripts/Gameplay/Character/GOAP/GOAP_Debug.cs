#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GOAP_Debug : MonoBehaviour
{
	[SerializeField] private List<Goal_Base> Goals = new();
	public Rect displayRect = new Rect(10, 10, 300, 20);
	[SerializeField] Sprite[] ActionImages = new Sprite[4];
	[SerializeField] Image ActionImage;

	private void Awake()
	{
		Goal_Base[] _goals = GetComponents<Goal_Base>();
		foreach (var goal in _goals)
		{
			Goals.Add(goal);
		}
	}
	// Change the sprite that appears above the characters head to reflect their current action
	// 0 - Idle
	// 1 - Wander
	// 2 - Explore
	// 3 - Loot
	// 4 - Engage
	// 5 - Attack
	// 6 - Celebrate
	// 7 - Death
	public void ChangeActionImage(int action)
	{
		ActionImage.sprite = ActionImages[action];
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
}
#endif
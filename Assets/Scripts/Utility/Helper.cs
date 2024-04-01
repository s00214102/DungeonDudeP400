using UnityEngine;

static public class Helper
{
	/// <summary>
	/// Pass two positions and a range. Returns true if the distance between them is less than the range.
	/// </summary>
	/// <param name="pos1"></param>
	/// <param name="pos2"></param>
	/// <param name="range"></param>
	/// <returns></returns>
	public static bool InRange(Vector3 pos1, Vector3 pos2, float range)
	{
		float dist = Vector3.Distance(pos1, pos2);
		return dist <= range;
	}
	public static void SetChildrenActive(GameObject parentObject, bool isActive)
	{
		foreach (Transform child in parentObject.transform)
		{
			child.gameObject.SetActive(isActive);
			SetChildrenActive(child.gameObject, isActive); // Recursively call for all children
		}
	}
	public static void DisableGameObject(GameObject gameObject)
	{
		CancelInvokesAndCoroutinesRecursively(gameObject);
		gameObject.SetActive(false);
	}
	// Function to cancel invokes and stop coroutines on all components of a GameObject and its children
	public static void CancelInvokesAndCoroutinesRecursively(GameObject gameObject)
	{
		// Cancel invokes on all components of the GameObject
		foreach (var component in gameObject.GetComponents<Component>())
		{
			if (component is MonoBehaviour monoBehaviour)
			{
				monoBehaviour.CancelInvoke();
				monoBehaviour.StopAllCoroutines();
			}
		}
		// Recursively cancel invokes and stop coroutines on children
		foreach (Transform child in gameObject.transform)
		{
			CancelInvokesAndCoroutinesRecursively(child.gameObject);
		}
	}
}
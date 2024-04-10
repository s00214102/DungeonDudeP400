using UnityEngine;

public class DungeonBlock : MonoBehaviour
{
	DungeonNavigationSystem navigation;

	private void Awake()
	{

	}
	private void Start()
	{
		navigation = GameObject.Find("DungeonNavigation").GetComponent<DungeonNavigationSystem>();
		navigation.MarkCellAsNotWalkable(this.transform.position);
	}
}
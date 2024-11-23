using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCharacterManager : MonoBehaviour
{
	internal DungeonNavigationSystem navigationSystem;
	[SerializeField] private List<GameObject> characters = new List<GameObject>();


	public void Init(DungeonNavigationSystem navigationSystem)
	{
		Debug.Log("Initializing DungeonCharacterManager.");

		this.navigationSystem = navigationSystem;

		DungeonNavAgentExploreMove[] agents = GetComponentsInChildren<DungeonNavAgentExploreMove>();

		foreach (var agent in agents)
		{
			agent.Init(this, navigationSystem);
			characters.Add(agent.gameObject);
		}
	}

	private void Start()
	{
		// Add all child gameobjects to the list

		// foreach (Transform child in transform)
		// {
		// 	DungeonCharacterMovement characterMovement;
		// 	if (child.TryGetComponent<DungeonCharacterMovement>(out characterMovement))
		// 	{
		// 		characters.Add(child.gameObject);
		// 	}
		// }
	}

	public void UpdateCharacterPosition(GameObject character)
	{
		var result = navigationSystem.UpdateCharacterPosition(character);
		if (result.successful)
		{
			character.GetComponent<DungeonNavAgentExploreMove>().StartBoundsCheck(result.position);
			// the agent has entered a new cell, add the current cells seeable cells to the agents list of known cells
			character.GetComponent<DungeonNavAgentExploreMove>().UpdateKnownCells(
				navigationSystem.dungeonGrid[result.position.x, result.position.y].SeeableCells);
		}
	}

	private void FixedUpdate()
	{
		// Call the Update function of each gameobject in the list
		for (int i = 0; i < characters.Count; i++)
		{
			if (characters[i] != null)
			{
				// Update characters positions
				//obj.SendMessage("DoFixedUpdate", SendMessageOptions.DontRequireReceiver);
				characters[i].GetComponent<DungeonNavAgentExploreMove>().DoFixedUpdate();

				// Update characters navigation system position
				UpdateCharacterPosition(characters[i]);
			}
		}
		// foreach (GameObject obj in characters)
		// {
		// 	if (obj != null)
		// 	{
		// 		// Update characters positions
		// 		//obj.SendMessage("DoFixedUpdate", SendMessageOptions.DontRequireReceiver);
		// 		obj.GetComponent<DungeonCharacterMovement>().DoFixedUpdate();

		// 		// Update characters navigation system position
		// 		var result = navigationSystem.UpdateCharacterPosition(obj);
		// 		if (result.successful)
		// 			obj.GetComponent<DungeonCharacterMovement>().StartBoundsCheck(result.position);
		// 	}
		// }
	}
	// called by a child character when they leave a cell, this is passed on to the nav system
	// public void CharacterLeftCell(Vector3Int cellPosition, GameObject character)
	// {
	// 	navigationSystem.RemoveCharacterFromCellPosition(cellPosition, character);
	// }
}
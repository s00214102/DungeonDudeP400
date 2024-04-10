using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCharacterManager : MonoBehaviour
{
	internal DungeonNavigationSystem navigationSystem;
	[SerializeField] private List<GameObject> characters = new List<GameObject>();

	private void Awake()
	{
		navigationSystem = GameObject.Find("DungeonNavigation").GetComponent<DungeonNavigationSystem>();
	}
	private void Start()
	{
		// Add all child gameobjects to the list
		foreach (Transform child in transform)
		{
			DungeonCharacterMovement characterMovement;
			if (child.TryGetComponent<DungeonCharacterMovement>(out characterMovement))
			{
				characters.Add(child.gameObject);
				// characterMovement.manager = this;
				// characterMovement.navigationSystem = navigationSystem;
				// characterMovement.cellSize = navigationSystem.cellSize;
			}
		}
	}
	public void UpdateCharacterPosition(GameObject character)
	{
		var result = navigationSystem.UpdateCharacterPosition(character);
		if (result.successful)
			character.GetComponent<DungeonCharacterMovement>().StartBoundsCheck(result.position);
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
				characters[i].GetComponent<DungeonCharacterMovement>().DoFixedUpdate();

				// Update characters navigation system position
				var result = navigationSystem.UpdateCharacterPosition(characters[i]);
				if (result.successful)
					characters[i].GetComponent<DungeonCharacterMovement>().StartBoundsCheck(result.position);
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
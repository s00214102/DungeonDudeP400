using System.Collections;
using Codice.CM.WorkspaceServer.DataStore.Merge;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// allows the player to setup the dungeon before the gameplay starts
public class DungeonSetupPhase : MonoBehaviour
{
	//public GameplayControls controls;
	public GameplayController gameplayController;

	private void Awake()
	{
		this.gameObject.SetActive(true);
		//controls = new GameplayControls();

		//moveVector = controls.Gameplay.MovePlayer.ReadValue<Vector3>();
		//controls.Gameplay.Interact.performed += OnInteract;
	}

	public GameObject updateCube;
	public GameObject invokeCube;
	public GameObject coroutineCube;
	// custom start method, called when the GameplayController activates this phase
	public void Begin()
	{
		InvokeRepeating("TestInvoke", 0, 0.1f);
		StartCoroutine(TestCoroutine());
		Helper.SetChildrenActive(this.gameObject, true);
	}
	private void TestInvoke()
	{
		invokeCube.transform.Rotate(new Vector3(0, 25, 0));
	}
	private IEnumerator TestCoroutine()
	{
		while (true)
		{
			coroutineCube.transform.Rotate(new Vector3(0, 10, 0));
			yield return null;
		}
	}
	private void Update()
	{
		updateCube.transform.Rotate(new Vector3(0, 10, 0));
	}
}
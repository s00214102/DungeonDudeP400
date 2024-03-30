using System.Collections;
using Codice.CM.WorkspaceServer.DataStore.Merge;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// allows the player to setup the dungeon before the gameplay starts
public class DungeonSetupPhase : MonoBehaviour
{
	//public GameplayControls controls;
	public GameplayController gameplayController;
	//TODO implement prefab selection from UI

	[SerializeField] private GameObject zombie_Prefab;
	[SerializeField] private GameObject hellhound_Prefab;
	[SerializeField] private GameObject ogre_Prefab;
	[SerializeField] private GameObject cannon_Prefab;
	[SerializeField] private GameObject barrier_Prefab;
	[SerializeField] private GameObject spikes_Prefab;
	[SerializeField] private GameObject tar_Prefab;
	[SerializeField] private GameObject snare_Prefab;
	[SerializeField] private GameObject fear_totem_Prefab;
	[SerializeField] private GameObject mimic_Prefab;
	[SerializeField] private GameObject selectedPrefab; // The currently selected prefab
	private Camera cam;
	private void Awake()
	{
		this.gameObject.SetActive(true);
		//controls = new GameplayControls();

		//moveVector = controls.Gameplay.MovePlayer.ReadValue<Vector3>();
		//controls.Gameplay.Interact.performed += OnInteract;

		cam = Camera.main;
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
		SpawnPrefabOnClick();
	}

	private void SpawnPrefabOnClick()
	{
		// Check if the left mouse button is clicked, and is not over a UI element
		if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
		{
			if (selectedPrefab != null)
			{
				// Get the mouse position in world coordinates
				Vector3 mousePosition = Mouse.current.position.ReadValue();
				Ray ray = cam.ScreenPointToRay(mousePosition);

				RaycastHit rayhit;

				if (Physics.Raycast(ray, out rayhit))
				{
					//Instantiate(selectedPrefab, rayhit.point, Quaternion.identity);
					NavMeshHit navhit;
					if (NavMesh.SamplePosition(rayhit.point, out navhit, Mathf.Infinity, NavMesh.AllAreas))
						Instantiate(selectedPrefab, navhit.position, Quaternion.identity);
				}

				//mousePosition = cam.ScreenToWorldPoint(mousePosition);
				//mousePosition.z = 0f; // Ensure the object is spawned at the same Z position as the camera

				// Instantiate the selected prefab at the mouse position
			}
		}
	}
	public enum monstersAndTraps
	{
		zombie, hellhound, ogre, cannon, barrier, spikes, tar, snare, fear_totem, mimic
	}
	//private monstersAndTraps currentSelection;
	public void ChoosePrefab(int value)
	{
		switch (value)
		{
			case 0:
				selectedPrefab = zombie_Prefab;
				break;
			case 1:
				selectedPrefab = hellhound_Prefab;
				break;
			case 2:
				selectedPrefab = ogre_Prefab;
				break;
			case 3:
				selectedPrefab = cannon_Prefab;
				break;
			case 4:
				selectedPrefab = barrier_Prefab;
				break;
			case 5:
				selectedPrefab = spikes_Prefab;
				break;
			case 6:
				selectedPrefab = tar_Prefab;
				break;
			case 7:
				selectedPrefab = snare_Prefab;
				break;
			case 8:
				selectedPrefab = fear_totem_Prefab;
				break;
			case 9:
				selectedPrefab = mimic_Prefab;
				break;
		}
	}
}
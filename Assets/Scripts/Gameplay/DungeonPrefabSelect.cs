using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DungeonPrefabSelect : MonoBehaviour
{
	// players energy, used to purchase things
	internal int startingEnergy;
	internal int energyRegenPerSecond;
	[SerializeField] private int currentEnergy;
	private int costToDeduce;
	[SerializeField] private TMP_Text txtEnergy;

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
	public RectTransform selectionMenuTransform;
	private Vector3 originalSelectionMenuPosition;
	private Vector3 hideSelectionMenuPosition;
	public GameObject btnHide;
	public GameObject btnShow;

	[SerializeField] private BoxCollider spawnArea;

	private void Start()
	{
		cam = Camera.main;

		if (selectionMenuTransform != null)
		{
			originalSelectionMenuPosition = selectionMenuTransform.anchoredPosition;
			hideSelectionMenuPosition = originalSelectionMenuPosition;
			hideSelectionMenuPosition.x -= 3000;
		}

		//LeanTween.move(selectionMenuTransform, new Vector3(0, 0, 0), 1);
		Helper.SetChildrenActive(this.gameObject, true);
		btnShow.SetActive(false);
		// set starting text for energy
		currentEnergy = startingEnergy;
		txtEnergy.text = currentEnergy.ToString();


	}
	private void Update()
	{
		SpawnPrefabOnClick();
	}

	private void SpawnPrefabOnClick()
	{
		// Check if the left mouse button is clicked, and is not over a UI element
		if (Mouse.current.leftButton.wasPressedThisFrame
		&& !EventSystem.current.IsPointerOverGameObject()
		&& CanAfford())
		{
			if (selectedPrefab != null)
			{
				// Get the mouse position in world coordinates
				Vector3 mousePosition = Mouse.current.position.ReadValue();
				Ray ray = cam.ScreenPointToRay(mousePosition);

				RaycastHit rayhit;

				if (Physics.Raycast(ray, out rayhit))
				{
					// is the rayhit point within the bounds of the box collider?
					if (spawnArea.bounds.Contains(rayhit.point))
					{
						//Instantiate(selectedPrefab, rayhit.point, Quaternion.identity);
						NavMeshHit navhit;
						if (NavMesh.SamplePosition(rayhit.point, out navhit, Mathf.Infinity, NavMesh.AllAreas))
						{
							Instantiate(selectedPrefab, navhit.position, Quaternion.identity);
							SpendEnergy();
						}
					}
					else
						ParticleManager.SpawnParticle(rayhit.point, ParticleManager.Particle.BloodSplatter);
				}

				//mousePosition = cam.ScreenToWorldPoint(mousePosition);
				//mousePosition.z = 0f; // Ensure the object is spawned at the same Z position as the camera

				// Instantiate the selected prefab at the mouse position
			}
		}
	}
	private void SpendEnergy()
	{
		currentEnergy -= costToDeduce;
		//update text field
		txtEnergy.text = currentEnergy.ToString();
	}
	public void GainEnergy(int amount)
	{
		currentEnergy += amount;
		txtEnergy.text = currentEnergy.ToString();
	}
	public void RegenEnergy()
	{
		GainEnergy(energyRegenPerSecond);
	}
	private bool CanAfford()
	{
		if (currentEnergy >= costToDeduce)
			return true;
		else
			return false;
	}
	public void HideSelectionMenu(bool value)
	{
		if (value)
		{
			//TODO use lean tween to move the selection menu down out of sight
			btnHide.SetActive(false);
			LeanTween.move(selectionMenuTransform, hideSelectionMenuPosition, 0.3f).setOnComplete(() => btnShow.SetActive(true));
		}
		else
		{
			//TODO use lean tween to move the selection menu back into sight
			btnShow.SetActive(false);
			LeanTween.move(selectionMenuTransform, originalSelectionMenuPosition, 0.3f).setOnComplete(() => btnHide.SetActive(true));
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
				costToDeduce = 20;
				break;
			case 1:
				selectedPrefab = hellhound_Prefab;
				costToDeduce = 40;
				break;
			case 2:
				selectedPrefab = ogre_Prefab;
				costToDeduce = 80;
				break;
			case 3:
				selectedPrefab = cannon_Prefab;
				costToDeduce = 60;
				break;
			case 4:
				selectedPrefab = barrier_Prefab;
				costToDeduce = 40;
				break;
			case 5:
				selectedPrefab = spikes_Prefab; costToDeduce = 30;
				break;
			case 6:
				selectedPrefab = tar_Prefab;
				costToDeduce = 60;
				break;
			case 7:
				selectedPrefab = snare_Prefab;
				costToDeduce = 80;
				break;
			case 8:
				selectedPrefab = fear_totem_Prefab;
				costToDeduce = 100;
				break;
			case 9:
				selectedPrefab = mimic_Prefab;
				costToDeduce = 200;
				break;
		}
	}
}
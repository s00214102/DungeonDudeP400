using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonPlayPhase : MonoBehaviour
{
	[SerializeField] private GameObject entrance;
	[SerializeField] private GameObject heroPrefab;

	[SerializeField] private HeroData[] heroClasses;

	[SerializeField] private HeroTraits[] heroTraits;

	[SerializeField] private List<GameObject> SpawnedHeroes = new();

	internal int heroesToSpawn = 10; // Number of heroes to spawn
	internal float spawnInterval = 1f; // Base spawn interval
	internal float spawnIntervalVariation = 1f; // Variation in spawn interval

	[HideInInspector] public GameplayController gameplayController;

	public void Begin()
	{
		Helper.SetChildrenActive(this.gameObject, true);
		StartCoroutine(SpawnHeroes());
		InvokeRepeating("RegenEnergy", 0, 1);
	}
	private void RegenEnergy()
	{
		gameplayController.prefabSelect.RegenEnergy();
	}
	private IEnumerator SpawnHeroes()
	{
		while (heroesToSpawn > 0)
		{
			SpawnHero();
			heroesToSpawn--;

			// Calculate next spawn time with variation
			float nextSpawnTime = spawnInterval + Random.Range(-spawnIntervalVariation, spawnIntervalVariation) + 0.1f;
			yield return new WaitForSeconds(nextSpawnTime);
		}
	}

	private void SpawnHero()
	{
		GameObject hero = Instantiate(heroPrefab, entrance.transform.position, Quaternion.identity);
		RandomizeHero(hero);
		SpawnedHeroes.Add(hero);
		Health heroHealth = hero.GetComponent<Health>();
		heroHealth.EntityDied.AddListener(() => RemoveDeadHero(hero));
		// player should gain some energy since the hero died
		// call DungeonPrefabSelect.GainEnergy through the gameplay controller
		heroHealth.EntityDied.AddListener(() => gameplayController.prefabSelect.GainEnergy(20));
	}

	private void RemoveDeadHero(GameObject hero)
	{
		// remove the dead hero from the list of heroes left to die
		SpawnedHeroes.Remove(hero);
		Health heroHealth = hero.GetComponent<Health>();
		//TODO check if RemoveAllListeners removes all listeners everywhere or just for this component
		heroHealth.EntityDied.RemoveAllListeners();
		WinConditionMet();
	}

	private void RandomizeHero(GameObject hero)
	{
		HeroData heroClass = heroClasses[Random.Range(0, heroClasses.Length)];
		HeroTraits heroTrait = heroTraits[Random.Range(0, heroTraits.Length)];
		GOAP_Hero_Data data = hero.GetComponent<GOAP_Hero_Data>();
		data.AssignClass(heroClass);
		data.AssignTrait(heroTrait);
	}
	public UnityEvent OnWinConditionMet;
	private void WinConditionMet()
	{
		if (heroesToSpawn == 0 && SpawnedHeroes.Count <= 0)
			OnWinConditionMet?.Invoke();
	}
}
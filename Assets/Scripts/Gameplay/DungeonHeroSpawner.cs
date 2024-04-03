using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonHeroSpawner : MonoBehaviour
{
	[SerializeField] private GameObject entrance;
	[SerializeField] private GameObject heroPrefab;

	[SerializeField] private HeroData[] heroClasses;

	[SerializeField] private HeroTraits[] heroTraits;

	[SerializeField] private List<GameObject> SpawnedHeroes = new();

	public int heroesToSpawn = 10; // Number of heroes to spawn
	public float spawnInterval = 1f; // Base spawn interval
	public float spawnIntervalVariation = 1f; // Variation in spawn interval

	public void Begin()
	{
		StartCoroutine(SpawnHeroes());
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
		heroHealth.OnDied.AddListener(() => RemoveDeadHero(hero));
	}

	private void RemoveDeadHero(GameObject hero)
	{
		SpawnedHeroes.Remove(hero);
		Health heroHealth = hero.GetComponent<Health>();
		//TODO check if RemoveAllListeners removes all listeners everywhere or just for this component
		heroHealth.OnDied.RemoveAllListeners();
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
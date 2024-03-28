using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class goap_flee
{
	private GameObject hero;
    private GameObject enemy;
    private Health enemy_health;
	private HeroKnowledge knowledge;
	private Inventory inventory;
	private Goal_Loot lootGoal;
	private Action_Loot lootAction;
	private GOAP_Hero_Data data;
	private PlayTestHelper testHelper;
	private GOAP_Planner planner;
	private HeroTraits greedyTrait;
	private GameObject treasureObj;
	private Treasure treasure;

	private IEnumerator Setup()
	{
		var operation = SceneManager.LoadSceneAsync("TestHeroGoalFlee", LoadSceneMode.Single);
		float maxLoadingTime = 10f;
		float startTime = Time.time;
		// Wait until the scene is fully loaded or timeout occurs
		while (!operation.isDone)
		{
			// Check if the loading operation exceeds the maximum waiting time
			if (Time.time - startTime >= maxLoadingTime)
			{
				Debug.LogError("Scene loading timed out.");
				yield break; // Exit the coroutine
			}
			yield return null;
		}

		hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

        enemy = GameObject.Find("FearTotem");
        Assert.IsNotNull(enemy, $"GameObject with name FearTotem not found.");

		enemy_health = enemy.GetComponent<Health>();
		Assert.IsNotNull(enemy_health, $"enemy_health component not found.");

		//lootGoal = hero.GetComponent<Goal_Flee>();
		//Assert.IsNotNull(lootGoal, $"Goal_Loot component not found.");

		//lootAction = hero.GetComponent<Action_Loot>();
		//Assert.IsNotNull(lootAction, $"Action_Loot component not found.");

		// data = hero.GetComponent<GOAP_Hero_Data>();
		// Assert.IsNotNull(data, $"GOAP_Hero_Data component not found.");

		// planner = hero.GetComponent<GOAP_Planner>();
		// Assert.IsNotNull(planner, $"GOAP_Planner component not found.");

		// testHelper = GameObject.Find("TestHelper").GetComponent<PlayTestHelper>();
		// Assert.IsNotNull(testHelper, $"PlayTestHelper not found.");

		// greedyTrait = testHelper.greedyPersonality;
		// Assert.IsNotNull(greedyTrait, $"Greedy trait not found.");

		// data.AssignTrait(greedyTrait);
		// Assert.IsTrue(data.HeroTraits == greedyTrait, "Heroes current trait is not the greedy trait.");

		// treasureObj = GameObject.Find("Treasure");
		// Assert.IsNotNull(treasureObj, $"GameObject with name Treasure not found.");

		// treasure = treasureObj.GetComponent<Treasure>();
		// Assert.IsNotNull(treasure, $"Treasure component not found.");

		yield break;
	}
	[UnityTest]
	public IEnumerator default_hero_flees_when_scared()
	{
		yield return Setup();

        //TODO finish this test
        // SETUP
        // scary enemy with lots of health so they dont die
        // hero stands next to them
        // their fear increases
        // this increases the flee goals priority
        // the priority gets high enough they take flee action
        // they go to the dungeon entrance

        // give the enemy very high hp
        enemy_health.SetMaxHealth(1000);

		Time.timeScale = 10.0f;
		float time = 0;
		while (time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;

		Assert.IsTrue(false, "condition not met.");
	}
}

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
	private Entity enemy_entity;
	private EntityProximityDetection enemy_detection;
	private HeroStatus heroStatus;
	private GOAP_Planner planner;
	private Goal_Flee goal_Flee;
	private Action_Flee action_Flee;
	private HeroKnowledge knowledge;

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

		heroStatus = hero.GetComponent<HeroStatus>();
		Assert.IsNotNull(heroStatus, $"heroStatus component not found.");

		enemy = GameObject.Find("FearTotem");
		Assert.IsNotNull(enemy, $"GameObject with name FearTotem not found.");

		enemy_health = enemy.GetComponent<Health>();
		Assert.IsNotNull(enemy_health, $"enemy_health component not found.");

		enemy_detection = enemy.GetComponent<EntityProximityDetection>();
		Assert.IsNotNull(enemy_detection, "enemy_detection component not found.");

		enemy_entity = enemy.GetComponent<Entity>();
		Assert.IsNotNull(enemy_entity, "enemy_entity component not found.");

		goal_Flee = hero.GetComponent<Goal_Flee>();
		Assert.IsNotNull(goal_Flee, "goal_Flee component not found.");

		action_Flee = hero.GetComponent<Action_Flee>();
		Assert.IsNotNull(action_Flee, "action_Flee component not found.");

		planner = hero.GetComponent<GOAP_Planner>();
		Assert.IsNotNull(planner, $"GOAP_Planner component not found.");

		knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"knowledge component not found.");

		// give the enemy very high hp
		enemy_health.SetMaxHealth(1000);

		yield break;
	}
	[UnityTest]
	public IEnumerator default_hero_fear_increases_when_inside_fear_aura()
	{
		yield return Setup();
		// SETUP
		// scary enemy with lots of health so they dont die
		// hero stands next to them
		// their fear increases

		// make sure that the fear totem has detected the hero
		Time.timeScale = 10.0f;
		float time = 0;
		while (enemy_detection.Entities.Count == 0 && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(enemy_detection.Entities.Count > 0, "Enemy not detecting the hero.");

		// the heroes fear value increases over time
		Time.timeScale = 10.0f;
		time = 0;
		while (heroStatus.Fear == 0 && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(heroStatus.Fear > 0, "Heroes fear status did not increase.");
	}
	[UnityTest]
	public IEnumerator default_hero_flees_when_fear_is_high_enough()
	{
		yield return Setup();

		// SETUP
		// their fear increases
		// this increases the flee goals priority
		// the flee goal takes priority and the flee action is the active action
		Time.timeScale = 10.0f;
		float time = 0;
		while (!(planner.ActiveAction is Action_Flee) && time < 30)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(planner.ActiveAction is Action_Flee, "Flee action was not taken.");
	}
	[UnityTest]
	public IEnumerator default_hero_flees_to_the_entrance()
	{
		yield return Setup();

		// when the hero reaches the entrance they should leave the dungeon
		Time.timeScale = 10.0f;

		bool action_flee_finished = false;
		action_Flee.DungeonExited.AddListener(() => { action_flee_finished = true; });

		Time.timeScale = 10.0f;
		float time = 0;
		while (!action_flee_finished && time < 30)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(action_flee_finished, "Hero didnt reach the entrance.");
	}
	[UnityTest]
	public IEnumerator hero_fear_lowers_passively()
	{
		//load an empty scene with only a hero in it
		SceneManager.LoadScene("TestEmpty", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.2f);

		hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		heroStatus = hero.GetComponent<HeroStatus>();
		Assert.IsNotNull(heroStatus, $"heroStatus component not found.");

		// manually add fear to the hero status
		heroStatus.AddFear(10);
		Time.timeScale = 10.0f;
		// does the heroes fear go back down to zero?
		float time = 0;
		while (heroStatus.Fear > 0 && time < 20)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(heroStatus.Fear == 0, "Hero fear didnt go back down to 0.");
	}
	[UnityTest]
	public IEnumerator hero_exits_dungeon()
	{
		yield return Setup();

		//TODO the hero should leave the dungeon when reaching the entrance
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

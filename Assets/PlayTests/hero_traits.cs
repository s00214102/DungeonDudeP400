//TODO upgrade to the new knowledge system
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class hero_traits
{
	//GameObject hero;
	// [SetUp]
	// public void SetUp()
	// {
	// 	SceneManager.LoadScene("TestHeroTraitGreed", LoadSceneMode.Single);
	// 	hero = GameObject.Find("GOAP_Hero");
	// 	Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");
	// }
	[UnityTest]
	public IEnumerator greedy_hero_prioritises_loot()
	{
		// Setup: the hero has the greedy personality trait (plunderer)
		// they are low hp
		// they know of an enemy, treasure, angel, and the goal
		// they should choose the treasure over all
		SceneManager.LoadScene("TestHeroTraitGreed", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.1f);

		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"HeroKnowledge component not found.");

		GameObject treasureObj = GameObject.Find("Treasure");
		Assert.IsNotNull(treasureObj, $"GameObject with name Treasure not found.");
		knowledge.RememberItem("Treasure", treasureObj, true);

		GameObject AngelObj = GameObject.Find("Angel");
		Assert.IsNotNull(AngelObj, $"GameObject with name Angel not found.");
		knowledge.RememberItem("Angel", AngelObj, true);

		GameObject EnemyObj = GameObject.Find("Enemy");
		Assert.IsNotNull(EnemyObj, $"GameObject with name Enemy not found.");
		Entity entity = EnemyObj.GetComponent<Entity>();
		Assert.IsNotNull(entity, "Entity component not found on Enemy");
		knowledge.RememberEnemy("Enemy", entity.gameObject, 100);

		GOAP_Hero_Data data = hero.GetComponent<GOAP_Hero_Data>();
		Assert.IsNotNull(data, $"GOAP_Hero_Data component not found.");

		PlayTestHelper testHelper = GameObject.Find("TestHelper").GetComponent<PlayTestHelper>();
		Assert.IsNotNull(testHelper, $"PlayTestHelper not found.");
		HeroTraits greedyTrait = testHelper.greedyPersonality;
		Assert.IsNotNull(greedyTrait, $"Plunderer trait not found.");

		data.AssignTrait(greedyTrait);
		Assert.IsTrue(data.HeroTraits == greedyTrait, "Heroes current trait is not the greedy trait.");

		Health health = hero.GetComponent<Health>();
		Assert.IsNotNull(health, "Health component not found on Hero.");
		// deal enough damage to the hero to make them half health
		health.TakeDamage(health.MaxHealth / 2);

		// does the hero prioritise the loot goal?
		GOAP_Planner planner = hero.GetComponent<GOAP_Planner>();
		Assert.IsNotNull(planner, $"GOAP_Planner component not found.");

		Time.timeScale = 10.0f;
		float time = 0;
		while (!(planner.ActiveAction is Action_Loot) && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(planner.ActiveAction is Action_Loot, "ActiveAction is not of type Action_Loot, the hero should prioritise looting over the other options since they have the plunderer personality type.");

		// make sure they looted
		Action_Loot loot_action = hero.GetComponent<Action_Loot>();
		Assert.IsNotNull(loot_action, $"Action_Loot component not found.");

		bool action_loot_finished = false;
		loot_action.Looted.AddListener(() => { action_loot_finished = true; });

		Time.timeScale = 10.0f;
		time = 0;
		while (!action_loot_finished && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(action_loot_finished, "Didnt finish looting");
	}
	[UnityTest]
	public IEnumerator aggressive_hero_prioritises_engaging_enemy()
	{
		// Setup: the hero has the greedy personality trait (plunderer)
		// they are low hp
		// they know of an enemy, treasure, angel, and the goal
		// they should choose the treasure over all
		SceneManager.LoadScene("TestHeroTraitGreed", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.1f);

		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"HeroKnowledge component not found.");

		GameObject treasureObj = GameObject.Find("Treasure");
		Assert.IsNotNull(treasureObj, $"GameObject with name Treasure not found.");
		knowledge.RememberItem("Treasure", treasureObj, true);

		GameObject AngelObj = GameObject.Find("Angel");
		Assert.IsNotNull(AngelObj, $"GameObject with name Angel not found.");
		knowledge.RememberItem("Angel", AngelObj, true);

		GameObject EnemyObj = GameObject.Find("Enemy");
		Assert.IsNotNull(EnemyObj, $"GameObject with name Enemy not found.");
		Health enemyHealth = EnemyObj.GetComponent<Health>();
		Assert.IsNotNull(enemyHealth, "Enemy health component not found.");
		Entity entity = EnemyObj.GetComponent<Entity>();
		Assert.IsNotNull(entity, "Entity component not found on Enemy");
		knowledge.RememberEnemy("Enemy", entity.gameObject, 100);

		GOAP_Hero_Data data = hero.GetComponent<GOAP_Hero_Data>();
		Assert.IsNotNull(data, $"GOAP_Hero_Data component not found.");

		PlayTestHelper testHelper = GameObject.Find("TestHelper").GetComponent<PlayTestHelper>();
		Assert.IsNotNull(testHelper, $"PlayTestHelper not found.");
		HeroTraits aggroTrait = testHelper.aggressivePersonality;
		Assert.IsNotNull(aggroTrait, $"Berserker trait not found.");

		data.AssignTrait(aggroTrait);
		Assert.IsTrue(data.HeroTraits == aggroTrait, "Heroes current trait is not the greedy trait.");

		Health health = hero.GetComponent<Health>();
		Assert.IsNotNull(health, "Health component not found on Hero.");
		// deal enough damage to the hero to make them half health
		health.TakeDamage(health.MaxHealth / 2);

		// does the hero prioritise the loot goal?
		GOAP_Planner planner = hero.GetComponent<GOAP_Planner>();
		Assert.IsNotNull(planner, $"GOAP_Planner component not found.");

		Time.timeScale = 10.0f;
		float time = 0;
		while (!(planner.ActiveAction is Action_Engage) && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(planner.ActiveAction is Action_Engage, "ActiveAction is not of type Action_Engage, the hero should prioritise engaging the enemy over the other options since they have the berserker personality type.");
	}
	[UnityTest]
	public IEnumerator cautious_hero_prioritises_healing()
	{
		// Setup: the hero has the greedy personality trait (plunderer)
		// they are low hp
		// they know of an enemy, treasure, angel, and the goal
		// they should choose the treasure over all
		SceneManager.LoadScene("TestHeroTraitGreed", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.1f);

		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"HeroKnowledge component not found.");

		GameObject treasureObj = GameObject.Find("Treasure");
		Assert.IsNotNull(treasureObj, $"GameObject with name Treasure not found.");
		knowledge.RememberItem("Treasure", treasureObj, true);

		GameObject AngelObj = GameObject.Find("Angel");
		Assert.IsNotNull(AngelObj, $"GameObject with name Angel not found.");
		knowledge.RememberItem("Angel", AngelObj, true);

		GameObject EnemyObj = GameObject.Find("Enemy");
		Assert.IsNotNull(EnemyObj, $"GameObject with name Enemy not found.");
		Health enemyHealth = EnemyObj.GetComponent<Health>();
		Assert.IsNotNull(enemyHealth, "Enemy health component not found.");
		Entity entity = EnemyObj.GetComponent<Entity>();
		Assert.IsNotNull(entity, "Entity component not found on Enemy");
		knowledge.RememberEnemy("Enemy", entity.gameObject, 100);

		GOAP_Hero_Data data = hero.GetComponent<GOAP_Hero_Data>();
		Assert.IsNotNull(data, $"GOAP_Hero_Data component not found.");

		PlayTestHelper testHelper = GameObject.Find("TestHelper").GetComponent<PlayTestHelper>();
		Assert.IsNotNull(testHelper, $"PlayTestHelper not found.");
		HeroTraits cowardTrait = testHelper.catiousPersonality;
		Assert.IsNotNull(cowardTrait, $"Coward trait not found.");

		data.AssignTrait(cowardTrait);
		Assert.IsTrue(data.HeroTraits == cowardTrait, "Heroes current trait is not the coward trait.");

		Health health = hero.GetComponent<Health>();
		Assert.IsNotNull(health, "Health component not found on Hero.");
		// deal enough damage to the hero to make them half health
		health.TakeDamage(health.MaxHealth / 2);

		// does the hero prioritise the heal goal?
		GOAP_Planner planner = hero.GetComponent<GOAP_Planner>();
		Assert.IsNotNull(planner, $"GOAP_Planner component not found.");

		Time.timeScale = 10.0f;
		float time = 0;
		while (!(planner.ActiveAction is Action_Heal) && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(planner.ActiveAction is Action_Heal, "ActiveAction is not of type Action_Heal, the hero should prioritise healing over the other options since they have the coward personality type.");
	}
	[UnityTest]
	public IEnumerator brave_hero_prioritises_attacking()
	{
		//hero with the fearless archetype (bravery 3)
		// should prioritise attacking over fleeing
		#region setup
		SceneManager.LoadScene("TestHeroGoalFlee", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.2f);

		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		HeroStatus heroStatus = hero.GetComponent<HeroStatus>();
		Assert.IsNotNull(heroStatus, $"heroStatus component not found.");

		GameObject enemy = GameObject.Find("FearTotem");
		Assert.IsNotNull(enemy, $"GameObject with name FearTotem not found.");

		Health enemy_health = enemy.GetComponent<Health>();
		Assert.IsNotNull(enemy_health, $"enemy_health component not found.");

		EntityProximityDetection enemy_detection = enemy.GetComponent<EntityProximityDetection>();
		Assert.IsNotNull(enemy_detection, "enemy_detection component not found.");

		Entity enemy_entity = enemy.GetComponent<Entity>();
		Assert.IsNotNull(enemy_entity, "enemy_entity component not found.");

		Goal_Flee goal_Flee = hero.GetComponent<Goal_Flee>();
		Assert.IsNotNull(goal_Flee, "goal_Flee component not found.");

		Action_Flee action_Flee = hero.GetComponent<Action_Flee>();
		Assert.IsNotNull(action_Flee, "action_Flee component not found.");

		GOAP_Planner planner = hero.GetComponent<GOAP_Planner>();
		Assert.IsNotNull(planner, $"GOAP_Planner component not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"knowledge component not found.");

		GOAP_Hero_Data data = hero.GetComponent<GOAP_Hero_Data>();
		Assert.IsNotNull(data, $"GOAP_Hero_Data component not found.");

		// give the enemy very high hp
		enemy_health.SetMaxHealth(1000);
		// set the heroes trait to fearless
		PlayTestHelper testHelper = GameObject.Find("TestHelper").GetComponent<PlayTestHelper>();
		Assert.IsNotNull(testHelper, $"PlayTestHelper not found.");
		HeroTraits braveTrait = testHelper.bravePersonality;
		Assert.IsNotNull(braveTrait, $"Brave trait not found.");

		data.AssignTrait(braveTrait);
		Assert.IsTrue(data.HeroTraits == braveTrait, "Heroes current trait is not the brave trait.");

		#endregion

		Time.timeScale = 10.0f;
		float time = 0;
		while (!(planner.ActiveAction is Action_Flee) && time < 20)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(!(planner.ActiveAction is Action_Flee), "The hero fled.");
	}
}
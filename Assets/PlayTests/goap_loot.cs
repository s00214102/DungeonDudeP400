using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class goap_loot
{
	private GameObject hero;
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
		var operation = SceneManager.LoadSceneAsync("TestHeroGoalLoot", LoadSceneMode.Single);
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

		knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"HeroKnowledge component not found.");

		inventory = hero.GetComponent<Inventory>();
		Assert.IsNotNull(inventory, $"Inventory component not found.");

		lootGoal = hero.GetComponent<Goal_Loot>();
		Assert.IsNotNull(lootGoal, $"Goal_Loot component not found.");

		lootAction = hero.GetComponent<Action_Loot>();
		Assert.IsNotNull(lootAction, $"Action_Loot component not found.");

		data = hero.GetComponent<GOAP_Hero_Data>();
		Assert.IsNotNull(data, $"GOAP_Hero_Data component not found.");

		planner = hero.GetComponent<GOAP_Planner>();
		Assert.IsNotNull(planner, $"GOAP_Planner component not found.");

		testHelper = GameObject.Find("TestHelper").GetComponent<PlayTestHelper>();
		Assert.IsNotNull(testHelper, $"PlayTestHelper not found.");

		greedyTrait = testHelper.greedyPersonality;
		Assert.IsNotNull(greedyTrait, $"Greedy trait not found.");

		data.AssignTrait(greedyTrait);
		Assert.IsTrue(data.HeroTraits == greedyTrait, "Heroes current trait is not the greedy trait.");

		treasureObj = GameObject.Find("Treasure");
		Assert.IsNotNull(treasureObj, $"GameObject with name Treasure not found.");

		treasure = treasureObj.GetComponent<Treasure>();
		Assert.IsNotNull(treasure, $"Treasure component not found.");


		yield break;
	}
	[UnityTest]
	public IEnumerator can_loot_treasure()
	{
		yield return Setup();

		treasure.randomlyGenerateLoot = false;
		treasure.AddItem("Ruby");

		// manually add the treasure object to the heroes memories
		knowledge.RememberItem("Treasure", treasureObj, true);
		// set the loot priority very high so the loot goal/action is chosen
		lootGoal.Priority = 999;
		// loot event from Action_Loot is used to know they finished looting
		bool action_loot_finished = false;
		lootAction.Looted.AddListener(() => { action_loot_finished = true; });
		// give enough time for the hero to get to the treasure and loot it
		// they should finish looting within the time
		Time.timeScale = 10.0f;
		float time = 0;
		while (!action_loot_finished && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;

		Assert.IsTrue(action_loot_finished, "Did not finish looting.");
		Assert.IsTrue(inventory.Items.Count > 0, "Hero inventory contains no items.");
	}
	[UnityTest]
	public IEnumerator remember_treasure()
	{
		// SETUP 
		// hero walks towards goal and encounters the treasure
		// the treasure is added as a memory in HeroKnowledge
		SceneManager.LoadScene("TestKnowledgeRememberTreasure", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.2f);
		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		// HeroProximityDetection is required for remembering things since they need to be detected first
		HeroProximityDetection detection = hero.GetComponent<HeroProximityDetection>();
		Assert.IsNotNull(detection, $"HeroProximityDetection component not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"HeroKnowledge component not found.");

		// wait for the hero to remember the treasure
		// when the treasure is detected, HeroProximityDetection will call knowledge.Remember("Treasure"...)
		Time.timeScale = 10.0f;
		float time = 0;
		while (knowledge.ItemMemories.Count == 0 && time < 5)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		ItemMemory memory = knowledge.ItemMemories.Find(memory => memory.ObjectType == "Treasure");
		Assert.IsNotNull(memory.ObjectType, "treasure memory memory.ObjectType  is null.");
		Assert.IsNotNull(memory.GameObject, "treasure memory memory.GameObject is null.");
	}
	// forget treasure after looting and finding it unuseable
	[UnityTest]
	public IEnumerator forget_treasure()
	{
		// SETUP: 
		// the hero should prioritise looting and go to the treasure
		// when the hero finishes looting and finds the treasure item not useable, they should forget it
		yield return Setup();

		// the treasure is programmatically added to the heroes memory
		// the treasure in this scene has no loot in it, so when the hero tries to loot from it they should remember it passing false
		knowledge.RememberItem("Treasure", treasureObj, true);

		// the hero prioritises looting and so moves to the treasures last known location
		lootGoal.Priority = 999;
		bool action_loot_finished = false;
		lootAction.Looted.AddListener(() => { action_loot_finished = true; });

		Time.timeScale = 10.0f;
		float time = 0;
		while (!action_loot_finished && time < 15)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(action_loot_finished, "Did not finish looting.");
		// there should only be one memory
		Assert.IsTrue(knowledge.ItemMemories.Count == 1, $"Unexpected number of item memories, {knowledge.ItemMemories.Count}.");
		// treasure memory bool should be false
		Assert.IsTrue(knowledge.ItemMemories[0].ItemIsUsable == false, $"Item is still useable, itemIsUsable should be false, but was {knowledge.ItemMemories[0].ItemIsUsable}.");
	}
	// Goal_Loot resets priority to 0 when finding the treasure no longer useable
	[UnityTest]
	public IEnumerator priority_reset_after_fail()
	{
		yield return Setup();

		// the treasure is programmatically added to the heroes memory
		// the treasure in this scene has no loot in it, so when the hero tries to loot from it they should remember it passing false
		knowledge.RememberItem("Treasure", treasureObj, true);

		// the hero prioritises looting and so moves to the treasures last known location
		lootGoal.Priority = 999;
		bool action_loot_finished = false;
		lootAction.Looted.AddListener(() => { action_loot_finished = true; });

		Time.timeScale = 10.0f;
		float time = 0;
		while (!action_loot_finished && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Assert.IsTrue(action_loot_finished, "Did not finish looting.");

		time = 0;
		while (lootGoal.Priority != 0 && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		// loot goal priority should be reset back to 0
		Assert.IsTrue(lootGoal.Priority == 0, $"Unexpected value: {lootGoal.Priority}, should be 0.");
	}
	// when using the last charge, the hero remembers that this treasure is no longer useable
	[UnityTest]
	public IEnumerator remember_not_useable_on_using_last_charge()
	{
		yield return Setup();

		treasure.randomlyGenerateLoot = false;
		treasure.AddItem("Ruby");

		// manually add the treasure object to the heroes memories
		knowledge.RememberItem("Treasure", treasureObj, true);
		// set the loot priority very high so the loot goal/action is chosen
		lootGoal.Priority = 999;
		// loot event from Action_Loot is used to know they finished looting
		bool action_loot_finished = false;
		lootAction.Looted.AddListener(() => { action_loot_finished = true; });

		Time.timeScale = 10.0f;
		float time = 0;
		while (!action_loot_finished && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		// when the hero uses the last charge of the treasure item, 
		// they should remember / set useable to false
		Assert.IsFalse(knowledge.ItemMemories[0].ItemIsUsable, "The treasure item is still useable, should have been set by not useable in Action_Loot.");

		// the loot action should not be taken again after looting has finished the first time
		Time.timeScale = 10.0f;
		time = 0;
		while ((planner.ActiveAction is Action_Loot) && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsFalse(planner.ActiveAction is Action_Loot, "ActiveAction is of type Action_Loot, the hero should not use this action again as there are no more viable options to loot in the scene.");
	}
	[UnityTest]
	public IEnumerator only_one_hero_loots_at_a_time()
	{
		//TODO: a treasure chest is 'in use' when a hero is using it
		// this stops other heroes from considering it
		yield return Setup();

		Time.timeScale = 10.0f;
		float time = 0;
		while (time < 5)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(false, "condition not met.");
	}
}

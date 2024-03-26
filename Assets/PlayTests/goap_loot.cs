using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class goap_loot
{

	[UnityTest]
	public IEnumerator can_loot_treasure()
	{
		// SETUP: get the hero and the loot goal component
		SceneManager.LoadScene("TestHeroGoalLoot", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.2f);

		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"HeroKnowledge component not found.");

		Inventory inventory = hero.GetComponent<Inventory>();
		Assert.IsNotNull(inventory, $"Inventory component not found.");

		Goal_Loot loot_goal = hero.GetComponent<Goal_Loot>();
		Assert.IsNotNull(loot_goal, $"Goal_Loot component not found.");

		Action_Loot loot_action = hero.GetComponent<Action_Loot>();
		Assert.IsNotNull(loot_action, $"Action_Loot component not found.");

		GameObject treasure = GameObject.Find("Treasure");
		Assert.IsNotNull(treasure, $"GameObject with name Treasure not found.");

		// manually add the treasure object to the heroes memories
		knowledge.RememberItem("Treasure", treasure, true);
		// set the loot priority very high so the loot goal/action is chosen
		loot_goal.Priority = 999;
		// loot event from Action_Loot is used to know they finished looting
		bool action_loot_finished = false;
		loot_action.Looted.AddListener(() => { action_loot_finished = true; });
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
		SceneManager.LoadScene("TestKnowledgeForgetTreasure", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.2f);

		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		GameObject treasure = GameObject.Find("Treasure");
		treasure.GetComponent<Treasure>().randomlyGenerateLoot = false;
		Assert.IsNotNull(treasure, $"GameObject with name Treasure not found.");

		HeroProximityDetection detection = hero.GetComponent<HeroProximityDetection>();
		Assert.IsNotNull(detection, $"HeroProximityDetection component not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"HeroKnowledge component not found.");

		Goal_Loot loot_goal = hero.GetComponent<Goal_Loot>();
		Assert.IsNotNull(loot_goal, $"Goal_Loot component not found.");

		Action_Loot loot_action = hero.GetComponent<Action_Loot>();
		Assert.IsNotNull(loot_action, $"Action_Loot component not found.");

		// the treasure is programmatically added to the heroes memory
		// the treasure in this scene has no loot in it, so when the hero tries to loot from it they should remember it passing false
		knowledge.RememberItem("Treasure", treasure, true);

		// the hero prioritises looting and so moves to the treasures last known location
		loot_goal.Priority = 999;
		bool action_loot_finished = false;
		loot_action.Looted.AddListener(() => { action_loot_finished = true; });

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
	//TODO test for loot goal priority reset
	[UnityTest]
	public IEnumerator priority_reset_after_fail()
	{
		#region Setup
		// the hero should prioritise looting and go to the treasure
		// when the hero finishes looting the loot goal priority should be back to 0
		SceneManager.LoadScene("TestKnowledgeForgetTreasure", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.2f);

		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		GameObject treasure = GameObject.Find("Treasure");
		treasure.GetComponent<Treasure>().randomlyGenerateLoot = false;
		Assert.IsNotNull(treasure, $"GameObject with name Treasure not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"HeroKnowledge component not found.");

		Goal_Loot loot_goal = hero.GetComponent<Goal_Loot>();
		Assert.IsNotNull(loot_goal, $"Goal_Loot component not found.");

		Action_Loot loot_action = hero.GetComponent<Action_Loot>();
		Assert.IsNotNull(loot_action, $"Action_Loot component not found.");
		#endregion

		// the treasure is programmatically added to the heroes memory
		// the treasure in this scene has no loot in it, so when the hero tries to loot from it they should remember it passing false
		knowledge.RememberItem("Treasure", treasure, true);

		// the hero prioritises looting and so moves to the treasures last known location
		loot_goal.Priority = 999;
		bool action_loot_finished = false;
		loot_action.Looted.AddListener(() => { action_loot_finished = true; });

		Time.timeScale = 10.0f;
		float time = 0;
		while (!action_loot_finished && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Assert.IsTrue(action_loot_finished, "Did not finish looting.");

		time = 0;
		while (loot_goal.Priority != 0 && time < 10)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		// loot goal priority should be reset back to 0
		Assert.IsTrue(loot_goal.Priority == 0, $"Unexpected value: {loot_goal.Priority}, should be 0.");
	}
	//TODO: as a hero if i remember multiple things of a type, i want to remember the closest one
	[UnityTest]
	public IEnumerator remembering_multiple_treasures_chooses_the_closest()
	{
		// SETUP
		SceneManager.LoadScene("test_scene_name", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.2f);
		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		HeroProximityDetection detection = hero.GetComponent<HeroProximityDetection>();
		Assert.IsNotNull(detection, $"HeroProximityDetection component not found.");

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
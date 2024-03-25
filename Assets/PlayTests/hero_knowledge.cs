using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class hero_knowledge
{
	// requirements
	// stores a list of entities (enemies) as well as the closest one
	// stores a list of Memories
	// can remember something, its type, its GameObject, and its last known location

	// as a hero i want to remember something when i find it
	// as a hero i want to recall where that thing when i need it
	// as a hero i want to forget that thing if i go back to where i think it was and its not there anymore
	// as a hero if i remember multiple things of a type, i want to remember the closest one
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
	// remembering the same treasure just updates its location
	// forget treasure
	[UnityTest]
	public IEnumerator forget_treasure()
	{
		// SETUP: the treasure is programmatically added to the heroes memory
		// then the treasure object  is deleted from the scene
		// the hero should prioritise looting and go to the treasure
		// when the hero reaches where the treasure was, they should forget it
		SceneManager.LoadScene("TestKnowledgeForgetTreasure", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.2f);

		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		GameObject treasure = GameObject.Find("Treasure");
		Assert.IsNotNull(treasure, $"GameObject with name Treasure not found.");

		HeroProximityDetection detection = hero.GetComponent<HeroProximityDetection>();
		Assert.IsNotNull(detection, $"HeroProximityDetection component not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"HeroKnowledge component not found.");

		knowledge.Remember("Treasure", treasure, treasure.transform.position, true);

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

	[UnityTest]
	public IEnumerator remember_enemy()
	{
		// SETUP 
		// hero walks towards goal and encounters the treasure
		// the treasure is added as a memory in HeroKnowledge
		SceneManager.LoadScene("TestKnowledgeRememberEnemy", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.2f);
		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		// HeroProximityDetection is required for remembering things since they need to be detected first
		HeroProximityDetection detection = hero.GetComponent<HeroProximityDetection>();
		Assert.IsNotNull(detection, $"HeroProximityDetection component not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"HeroKnowledge component not found.");

		Time.timeScale = 10.0f;
		float time = 0;
		while (knowledge.Entities.Count == 0 && time < 5)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		//Entity enemy = knowledge.Entities.Find(enemy => enemy. == "Treasure");

		Assert.IsTrue(knowledge.Entities.Count > 0, "enemy not found.");
	}

}
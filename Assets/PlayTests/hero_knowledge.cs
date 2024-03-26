using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class hero_knowledge
{
	[Test]
	public void remember_item()
	{
		// SETUP:
		HeroKnowledge heroKnowledge = new GameObject().AddComponent<HeroKnowledge>();
		GameObject testItem = new GameObject();
		// before adding an item the list should be empty
		Assert.IsTrue(heroKnowledge.ItemMemories.Count == 0);
		heroKnowledge.RememberItem("TestItem", testItem, true);
		// after remembering there should be one item in the list
		Assert.IsTrue(heroKnowledge.ItemMemories.Count == 1);
	}
	[Test]
	public void forget_item()
	{
		// SETUP:
		HeroKnowledge heroKnowledge = new GameObject().AddComponent<HeroKnowledge>();
		GameObject testItem = new GameObject();
		// before adding an item the list should be empty
		Assert.IsTrue(heroKnowledge.ItemMemories.Count == 0, $"Memories count does not equal 0 to start. actual: {heroKnowledge.ItemMemories.Count}");
		heroKnowledge.RememberItem("TestItem", testItem, true);
		Assert.IsTrue(heroKnowledge.ItemMemories.Count == 1, $"Memories count doesnt equal 1 after remembering the item. actual: {heroKnowledge.ItemMemories.Count}");
		heroKnowledge.ForgetItem(testItem);
		// after 'forgetting' the item, its usable bool should be false
		Assert.IsTrue(heroKnowledge.ItemMemories[0].ItemIsUsable == false, $"the item memories itemIsUsable bool should be false, actual value: {heroKnowledge.ItemMemories[0].ItemIsUsable}");
	}
	[Test]
	public void recall_first_item_position()
	{
		// SETUP:
		HeroKnowledge heroKnowledge = new GameObject().AddComponent<HeroKnowledge>();
		GameObject testItem = new GameObject();
		testItem.transform.position = new Vector3(1, 1, 1); // validate this position in assert

		heroKnowledge.RememberItem("TestItem", testItem, true);
		var result = heroKnowledge.RecallFirstItemPosition("TestItem");

		Assert.IsTrue(result.found, "method result was false");
		Assert.IsTrue(result.position == new Vector3(1, 1, 1), "position is not the same");
	}
	[Test]
	public void recall_closest_item()
	{
		// SETUP:
		GameObject hero = new GameObject();
		hero.transform.position = new Vector3(0, 0, 0);
		HeroKnowledge heroKnowledge = hero.AddComponent<HeroKnowledge>();

		GameObject testItem1 = new GameObject();
		testItem1.transform.position = new Vector3(1, 1, 1); // the closest position

		GameObject testItem2 = new GameObject();
		testItem2.transform.position = new Vector3(2, 2, 2);

		GameObject testItem3 = new GameObject();
		testItem3.transform.position = new Vector3(3, 3, 3);

		// add items in reverse order so the farthest is added first, then the closest
		heroKnowledge.RememberItem("TestItem", testItem3, true);
		heroKnowledge.RememberItem("TestItem", testItem1, true);
		heroKnowledge.RememberItem("TestItem", testItem2, true);
		var result = heroKnowledge.RecallClosestItem("TestItem");

		Assert.IsTrue(result.found, "method result was false");
		Assert.IsNotNull(result.item, "returned item was null");
		Assert.IsTrue(heroKnowledge.ItemMemories.Count == 3, $"unexpected number of item memories: {heroKnowledge.ItemMemories.Count}");
		Assert.IsTrue(result.item.gameObject.transform.position == new Vector3(1, 1, 1), $"position was not the closest one: {result.item.gameObject.transform.position}");
	}
	[Test]
	public void recall_closest_item_one_item()
	{
		// SETUP:
		GameObject hero = new GameObject();
		hero.transform.position = new Vector3(0, 0, 0);
		HeroKnowledge heroKnowledge = hero.AddComponent<HeroKnowledge>();

		GameObject testItem1 = new GameObject();
		testItem1.transform.position = new Vector3(1, 1, 1); // the closest position

		heroKnowledge.RememberItem("TestItem", testItem1, true);
		var result = heroKnowledge.RecallClosestItem("TestItem");

		Assert.IsTrue(result.found, "method result was false");
		Assert.IsNotNull(result.item, "returned item was null");
		Assert.IsTrue(heroKnowledge.ItemMemories.Count == 1, $"unexpected number of item memories: {heroKnowledge.ItemMemories.Count}");
		Assert.IsTrue(result.item.gameObject.transform.position == new Vector3(1, 1, 1), $"position was not the closest one: {result.item.gameObject.transform.position}");
	}
	[Test]
	public void recall_closest_item_no_items()
	{
		// SETUP:
		GameObject hero = new GameObject();
		HeroKnowledge heroKnowledge = hero.AddComponent<HeroKnowledge>();

		var result = heroKnowledge.RecallClosestItem("TestItem");

		Assert.IsTrue(!result.found, "method result was true");
		Assert.IsNull(result.item, "returned item was not null");
		Assert.IsTrue(heroKnowledge.ItemMemories.Count == 0, $"unexpected number of item memories: {heroKnowledge.ItemMemories.Count}");
	}
	[Test]
	public void recall_first_usable_item()
	{
		// SETUP:
		HeroKnowledge heroKnowledge = new GameObject().AddComponent<HeroKnowledge>();
		GameObject testItem = new GameObject();
		testItem.transform.position = new Vector3(1, 1, 1); // validate this position in assert

		heroKnowledge.RememberItem("TestItem", testItem, true);
		var result = heroKnowledge.RecallFirstUsableItem("TestItem");

		Assert.IsTrue(result.found, "method result was false");
		Assert.IsTrue(result.item.ItemIsUsable == true, "item is not useable");
	}
	[Test]
	public void recall_first_usable_item_where_not_useable()
	{
		// it shouldnt return false and null since the item we remember is not useable
		HeroKnowledge heroKnowledge = new GameObject().AddComponent<HeroKnowledge>();
		GameObject testItem = new GameObject();
		testItem.transform.position = new Vector3(1, 1, 1); // validate this position in assert

		heroKnowledge.RememberItem("TestItem", testItem, false);
		var result = heroKnowledge.RecallFirstUsableItem("TestItem");

		Assert.IsTrue(!result.found, "method result was false");
		Assert.IsNull(result.item, "item is not null");
	}
	[Test]
	public void recall_closest_useable_item()
	{
		// SETUP:
		GameObject hero = new GameObject();
		hero.transform.position = new Vector3(0, 0, 0);
		HeroKnowledge heroKnowledge = hero.AddComponent<HeroKnowledge>();

		GameObject testItem1 = new GameObject();
		testItem1.transform.position = new Vector3(1, 1, 1); // the closest position

		GameObject testItem2 = new GameObject();
		testItem2.transform.position = new Vector3(2, 2, 2); // the closest useable

		GameObject testItem3 = new GameObject();
		testItem3.transform.position = new Vector3(3, 3, 3);

		heroKnowledge.RememberItem("TestItem", testItem3, true);
		heroKnowledge.RememberItem("TestItem", testItem1, false); // make it unuseable
		heroKnowledge.RememberItem("TestItem", testItem2, true);
		var result = heroKnowledge.RecallClosestUsableItem("TestItem");

		Assert.IsTrue(result.found, "method result was false");
		Assert.IsNotNull(result.item, "returned item was null");
		Assert.IsTrue(heroKnowledge.ItemMemories.Count == 3, $"unexpected number of item memories: {heroKnowledge.ItemMemories.Count}");
		Assert.IsTrue(result.item.GameObject.transform.position == new Vector3(2, 2, 2), $"position was not the closest one: {result.item.GameObject.transform.position}");
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
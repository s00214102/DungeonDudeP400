using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class hero_goals
{
	[SetUp]
	public void SetUp()
	{
		//SceneManager.LoadScene("TestHealth", LoadSceneMode.Single);
	}
	[UnityTest]
	public IEnumerator can_loot_treasure()
	{
		// SETUP: get the hero and the loot goal component
		// set the loot priority very high
		// give enough time for the hero to get to the treasure and loot it
		// they should finish looting within the time
		// some response is needed to know they finished looting
		SceneManager.LoadScene("TestHeroGoalLoot", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.2f);

		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		Goal_Loot loot_goal = hero.GetComponent<Goal_Loot>();
		Assert.IsNotNull(loot_goal, $"Goal_Loot component not found.");

		loot_goal.Priority = 999;

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
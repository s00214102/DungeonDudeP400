using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class hero_proximity_detection
{
	[SetUp]
	public void SetUp()
	{
		SceneManager.LoadScene("TestHeroProximityDetection", LoadSceneMode.Single);
	}

	[UnityTest]
	public IEnumerator finds_closest_target_among_many()
	{
		GameObject hero = GameObject.Find("GOAP_Hero");
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		HeroProximityDetection detection = hero.GetComponent<HeroProximityDetection>();
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(hero, $"HeroProximityDetection component not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(hero, $"HeroKnowledge component not found.");

		GameObject closestEnemy = GameObject.Find("ClosestEnemy");
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(hero, $"GameObject with name ClosestEnemy not found.");

		// is the closest enemy the one we expected?
		bool result = knowledge.closestTarget.gameObject == closestEnemy;
		Assert.IsTrue(result, "The closest enemy found was not what was expected.");
	}
}
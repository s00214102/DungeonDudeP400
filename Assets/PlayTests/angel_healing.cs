using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class angel_healing
{
	private GameObject hero;
	[SetUp]
	public void SetUp()
	{
		SceneManager.LoadScene("TestAngelHealing", LoadSceneMode.Single);
		hero = GameObject.FindWithTag("Hero");
	}
	[UnityTest]
	public IEnumerator remembers_angel_and_heals_when_low_hp()
	{
		hero = GameObject.FindWithTag("Hero");
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(hero, $"GameObject with tag Hero not found.");

		GOAP_Planner planner = hero.GetComponent<GOAP_Planner>();
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(planner, $"GOAP_Planner component not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(planner, $"HeroKnowledge component not found.");

		GameObject angel = knowledge.RecallObjectByName("Angel");
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(planner, $"Angel memory not found.");

		Health health = hero.GetComponent<Health>();
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(planner, $"Health component not found.");

		Time.timeScale = 10.0f;
		float time = 0;
		bool hpDropped = false;
		while (hpDropped && (health.CurrentHealth > 20 || time < 5))
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		hpDropped = true;
		Assert.IsTrue(health.CurrentHealth <= 20, $"Health did not drop low. {health.CurrentHealth}");

		// heal action is taken
		//Assert.IsTrue(planner.ActiveAction is Action_Heal, $"Heal action not taken. {planner.ActiveAction}");

		// health is full again
		time = 0;
		while (health.CurrentHealth != health.MaxHealth || time < 5)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		// Reset timeScale
		Time.timeScale = 1.0f;
		Assert.IsTrue(health.CurrentHealth == health.MaxHealth, $"Health not full. {health.CurrentHealth}");
	}
}
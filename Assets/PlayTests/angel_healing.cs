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
		Assert.IsNotNull(hero, $"GameObject with tag Hero not found.");

		GOAP_Planner planner = hero.GetComponent<GOAP_Planner>();
		Assert.IsNotNull(planner, $"GOAP_Planner component not found.");

		HeroKnowledge knowledge = hero.GetComponent<HeroKnowledge>();
		Assert.IsNotNull(knowledge, $"HeroKnowledge component not found.");

		Health health = hero.GetComponent<Health>();
		Assert.IsNotNull(health, $"Health component not found.");

		Goal_Heal heal_goal = hero.GetComponent<Goal_Heal>();
		Assert.IsNotNull(heal_goal, $"Goal_Heal component not found.");

		Action_Heal action_Heal = hero.GetComponent<Action_Heal>();
		Assert.IsNotNull(action_Heal, $"Action_Heal component not found.");

		bool action_heal_finished = false;
		action_Heal.Healed.AddListener(() => { action_heal_finished = true; });

		Time.timeScale = 3.0f;
		float time = 0;
		bool hpDropped = false;
		while (!hpDropped && (health.CurrentHealth > 30 || time < 15))
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		hpDropped = true;
		Assert.IsTrue(health.CurrentHealth <= 20, $"Health did not drop low. {health.CurrentHealth}");

		var result = knowledge.RecallClosestUsableItem("Angel");
		Assert.IsTrue(result.found, $"Angel memory not found.");

		GameObject angel = result.item.GameObject;
		Assert.IsNotNull(angel, $"Angel gameobject is null.");

		// heal action is taken - planner.ActiveAction is Action_Heal
		Assert.IsTrue(heal_goal.Priority > 0, $"Heal priority not greater than 0. {heal_goal.Priority}");

		// health is full again
		time = 0;
		while (health.CurrentHealth != health.MaxHealth && time < 30)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		// Reset timeScale
		Time.timeScale = 1.0f;
		Assert.IsTrue(health.CurrentHealth == health.MaxHealth, $"Health not full. {health.CurrentHealth}");
		Assert.IsTrue(action_heal_finished, $"Healed event didnt fire.");
	}
}
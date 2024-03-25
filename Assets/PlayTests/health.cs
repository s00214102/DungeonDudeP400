using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class health
{
	[SetUp]
	public void SetUp()
	{

	}

	[UnityTest]
	public IEnumerator can_take_damage()
	{
		// checks that the health component can take damage
		SceneManager.LoadScene("TestHealthDamage", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.5f);
		GameObject hero = GameObject.Find("GOAP_Hero");
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		Health health = hero.GetComponent<Health>();
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(hero, $"Health component not found.");

		Time.timeScale = 10.0f;
		float time = 0;
		while (health.CurrentHealth == health.MaxHealth && time < 5)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(health.CurrentHealth < health.MaxHealth, "Health component did not take damage.");
	}
	[UnityTest]
	public IEnumerator can_die()
	{
		// tests that the health component "can die"
		// the EntityDied event should trigger
		// the isDead bool should be true
		SceneManager.LoadScene("TestHealthDie", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.5f);
		GameObject hero = GameObject.Find("GOAP_Hero");
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		Health health = hero.GetComponent<Health>();
		yield return new WaitForSeconds(0.1f);
		Assert.IsNotNull(hero, $"Health component not found.");

		bool died = false;
		health.EntityDied.AddListener(() => { died = true; });

		Time.timeScale = 10.0f;
		float time = 0;
		while (health.CurrentHealth > 0 && time < 5)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(health.CurrentHealth <= 0, "Current hp not less than or equal to 0.");
		Assert.IsTrue(died, "EntityDied event did not fire.");
		Assert.IsTrue(health.isDead, "Health.isDead is not true.");
	}
}
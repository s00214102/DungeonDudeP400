using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class traps
{
	private GameObject hero;
	private GameObject barrier;

	[SetUp]
	public void SetUp()
	{
		SceneManager.sceneLoaded += MySetup;
	}
	[TearDown]
	public void TearDown()
	{
		SceneManager.sceneLoaded -= MySetup;
	}
	private void MySetup(Scene scene, LoadSceneMode mode)
	{
		hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		barrier = GameObject.Find("Barrier");
		Assert.IsNotNull(barrier, $"GameObject with name Barrier not found.");

		// give the hero the aggressive trait so he attacks
	}
	[UnityTest]
	public IEnumerator hero_attacks_barrier()
	{
		SceneManager.LoadScene("TestBarrier", LoadSceneMode.Single);
		// this scene doesnt contain a goal so that the hero wont prioritise the gotogoal action
		// instead make him manually move to a predefined position
		CharacterMovement movement = hero.GetComponent<CharacterMovement>();
		GameObject goal = GameObject.Find("FakeGoal");
		movement.MoveTo(goal.transform.position);

		Health barrierHealth = barrier.GetComponent<Health>();

		Time.timeScale = 10.0f;
		float time = 0;
		while (barrierHealth.CurrentHealth == barrierHealth.MaxHealth && time < 20)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = 1.0f;
		Assert.IsTrue(barrierHealth.CurrentHealth < barrierHealth.MaxHealth, "Barrier health did not drop.");
	}
}
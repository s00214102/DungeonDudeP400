using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class traps
{

	[UnityTest]
	public IEnumerator hero_attacks_barrier()
	{
		SceneManager.LoadScene("TestBarrier", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.1f);

		GameObject hero = GameObject.Find("GOAP_Hero");
		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

		GameObject barrier = GameObject.Find("Barrier");
		Assert.IsNotNull(barrier, $"GameObject with name Barrier not found.");
		// this scene doesnt contain a goal so that the hero wont prioritise the gotogoal action
		// instead make him manually move to a predefined position
		CharacterMovement movement = hero.GetComponent<CharacterMovement>();
		Assert.IsNotNull(movement, $"CharacterMovement component not found.");

		GameObject goal = GameObject.Find("FakeGoal");
		Assert.IsNotNull(goal, $"GameObject with name FakeGoal not found.");

		Health barrierHealth = barrier.GetComponent<Health>();
		Assert.IsNotNull(barrierHealth, $"Health component not found.");

		movement.MoveTo(goal.transform.position);

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
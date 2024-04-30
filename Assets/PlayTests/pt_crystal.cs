using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class pt_crystal
{
	[UnityTest]
	public IEnumerator crystal_takes_damage()
	{
        SceneManager.LoadScene("TestCrystal", LoadSceneMode.Single);
        yield return new WaitForSeconds(0.2f);

        GameObject crystal = GameObject.Find("Goal");
        Assert.IsNotNull(crystal, $"GameObject with tag Goal not found.");

        Health health = crystal.GetComponent<Health>();
        Assert.IsNotNull(health, $"Health component not found.");

        int halfHP = health.MaxHealth/2;

		Time.timeScale = 10.0f;
		float time = 0;
		while (health.CurrentHealth >= 100 || time < 30)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
        Time.timeScale = 1.0f;
		Assert.IsTrue(health.CurrentHealth <= 100, $"Health did not drop below half health. {health.CurrentHealth}");
	}
}
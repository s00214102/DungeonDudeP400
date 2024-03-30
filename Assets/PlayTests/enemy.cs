using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class enemy
{
	private GameObject hero;

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
	}
	[UnityTest]
	public IEnumerator turret()
	{
		SceneManager.LoadScene("TestTurret", LoadSceneMode.Single);
		yield return new WaitForSeconds(0.1f);
		Assert.IsTrue(false, "condition not met");
	}
}
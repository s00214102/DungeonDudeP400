// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.TestTools;

// public class hero_senses
// {
// 	private GameObject hero;
// 	private GameObject enemy;

// 	private IEnumerator Setup()
// 	{
// 		var operation = SceneManager.LoadSceneAsync("TestSenses", LoadSceneMode.Single);
// 		float maxLoadingTime = 10f;
// 		float startTime = Time.time;
// 		// Wait until the scene is fully loaded or timeout occurs
// 		while (!operation.isDone)
// 		{
// 			// Check if the loading operation exceeds the maximum waiting time
// 			if (Time.time - startTime >= maxLoadingTime)
// 			{
// 				Debug.LogError("Scene loading timed out.");
// 				yield break; // Exit the coroutine
// 			}
// 			yield return null;
// 		}

// 		hero = GameObject.Find("GOAP_Hero");
// 		Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

// 		enemy = GameObject.Find("enemy");
// 		Assert.IsNotNull(enemy, $"GameObject with name enemy not found.");

// 		yield break;
// 	}
// 	[UnityTest]
// 	public IEnumerator TestMethod()
// 	{
// 		yield return Setup();


// 	}
// }
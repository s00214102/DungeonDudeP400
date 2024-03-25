using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class test_scene
{
	[SetUp]
	public void SetUp()
	{
		SceneManager.LoadScene("TestLoadScene", LoadSceneMode.Single);
	}

	[UnityTest]
	public IEnumerator test_scene_is_loaded()
	{
		yield return null;
		Assert.IsTrue(SceneManager.GetSceneByName("TestLoadScene").isLoaded);
	}

	[UnityTest]
	public IEnumerator find_object_by_name()
	{
		yield return null;
		string gameObjectName = "TestObject";
		GameObject foundObject = GameObject.Find("TestObject");
		Assert.IsNotNull(foundObject, $"GameObject with name '{gameObjectName}' not found.");
	}
	[UnityTest]
	public IEnumerator find_object_by_tag()
	{
		yield return null;
		string gameObjectTag = "Enemy";
		GameObject foundObject = GameObject.FindGameObjectWithTag(gameObjectTag);
		Assert.IsNotNull(foundObject, $"GameObject with tag '{gameObjectTag}' not found.");
	}
	[TearDown]
	public void TearDown()
	{

	}
}
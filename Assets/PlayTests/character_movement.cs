using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class character_movement
{
    [SetUp]
    public void SetUp()
    {
        SceneManager.LoadScene("TestCharacterMovement", LoadSceneMode.Single);
    }
    [UnityTest]
    public IEnumerator can_reach_destination_via_navmesh()
    {
        // confirm its position is near enough to the finish object
        GameObject character = GameObject.Find("character");
        CharacterMovement movement = character.GetComponent<CharacterMovement>();
        GameObject finish = GameObject.Find("finish");
        bool destinationReached = false;
        movement.DestinationReached.AddListener(() => { destinationReached = true; });
        movement.MoveTo(finish.transform.position);
        Time.timeScale = 10.0f;
        float timeoutDuration = 10f; // Timeout duration in seconds
        float startTime = Time.time; // Record the start time

        // Wait until the destination is reached or timeout occurs
        while (!destinationReached && Time.time - startTime < timeoutDuration)
        {
            yield return null; // Wait for the next frame
        }
        Time.timeScale = 1f;
        Assert.IsTrue(destinationReached, "Destination was not reached in time.");
    }
}

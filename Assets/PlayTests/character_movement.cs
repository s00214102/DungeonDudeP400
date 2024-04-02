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
        GameObject hero = GameObject.Find("GOAP_Hero");
        Assert.IsNotNull(hero, $"GameObject with name GOAP_Hero not found.");

        CharacterMovement movement = hero.GetComponent<CharacterMovement>();
        Assert.IsNotNull(movement, $"CharacterMovement component not found.");

        GameObject finish = GameObject.Find("finish");
        Assert.IsNotNull(finish, $"GameObject with name finish not found.");

        bool destinationReached = false;
        movement.DestinationReached.AddListener(() => { destinationReached = true; });
        movement.MoveTo(finish.transform.position);

        Time.timeScale = 10.0f;
        float time = 0;
        while (!destinationReached && time < 20)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        Time.timeScale = 1f;
        Assert.IsTrue(destinationReached, "Destination was not reached in time.");
    }
}

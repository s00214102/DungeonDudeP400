using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// allows the player to setup the dungeon before the gameplay starts
public class DungeonSetupPhase : MonoBehaviour
{
	[SerializeField] private TMP_Text txtTime;
	internal float timeToCount;

	public UnityEvent TimerFinished;
	private void OnTimerFinished() { TimerFinished?.Invoke(); }

	//public GameplayControls controls;
	public GameplayController gameplayController;

	private void Awake()
	{
		this.gameObject.SetActive(true);
		//controls = new GameplayControls();

		//moveVector = controls.Gameplay.MovePlayer.ReadValue<Vector3>();
		//controls.Gameplay.Interact.performed += OnInteract;
	}

	public GameObject updateCube;
	public GameObject invokeCube;
	public GameObject coroutineCube;
	// custom start method, called when the GameplayController activates this phase
	public void Begin()
	{
		InvokeRepeating("TestInvoke", 0, 0.1f);
		StartCoroutine(TestCoroutine());
		Helper.SetChildrenActive(this.gameObject, true);
		// timer
		UpdateTimerText(timeToCount);
		StartCoroutine(Countdown(timeToCount));
	}
	private void TestInvoke()
	{
		invokeCube.transform.Rotate(new Vector3(0, 25, 0));
	}
	private IEnumerator TestCoroutine()
	{
		while (true)
		{
			coroutineCube.transform.Rotate(new Vector3(0, 10, 0));
			yield return null;
		}
	}
	private void Update()
	{
		updateCube.transform.Rotate(new Vector3(0, 10, 0));
	}
	IEnumerator Countdown(float duration)
	{
		float timeRemaining = duration;
		while (timeRemaining > 0)
		{
			yield return new WaitForSeconds(1f); // Wait for 1 second
			timeRemaining -= 1f; // Decrease time remaining by 1 second
			UpdateTimerText(timeRemaining); // Update the timer text
		}

		// Timer finished
		UpdateTimerText(0f); // Ensure the timer text displays 0
		OnTimerFinished(); // Invoke the timer finished event
	}
	void UpdateTimerText(float timeRemaining)
	{
		if (txtTime != null)
		{
			txtTime.text = Mathf.CeilToInt(timeRemaining).ToString(); // Update the text with rounded time remaining
		}
	}
}
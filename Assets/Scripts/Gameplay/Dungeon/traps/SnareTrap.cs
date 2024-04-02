using System.Collections;
using UnityEngine;

public class SnareTrap : Trap
{
	public float snareTime = 1f;
	public bool isSnared = false;

	protected override void OnTriggerEnter(Collider other)
	{
		CharacterMovement movementComponent;
		if (other.TryGetComponent(out movementComponent))
		{
			// pin a hero in place for X seconds
			// only one hero should be pinned at once
			if (!isSnared)
				StartCoroutine(SnareTarget(movementComponent));
		}
	}
	private IEnumerator SnareTarget(CharacterMovement movementComponent)
	{
		isSnared = true;
		//movementComponent.StopMoving();
		movementComponent.NoSpeed();
		yield return new WaitForSeconds(snareTime);
		movementComponent.ResetSpeed();
		isSnared = false;
		Disarm();
	}
}
using UnityEngine;

public class SnareTrap : Trap
{
	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		CharacterMovement movementComponent;
		if (other.TryGetComponent(out movementComponent))
		{
			//TODO pin a hero in place for X seconds
			//movementComponent.StopMovement()
		}
	}
}
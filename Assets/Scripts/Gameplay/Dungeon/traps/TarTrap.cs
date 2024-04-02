using UnityEngine;

public class TarTrap : Trap
{
	protected override void OnTriggerEnter(Collider other)
	{
		CharacterMovement movementComponent;
		if (other.TryGetComponent(out movementComponent))
		{
			movementComponent.SlowSpeed();
		}
	}
	protected override void OnTriggerExit(Collider other)
	{
		CharacterMovement movementComponent;
		if (other.TryGetComponent(out movementComponent))
		{
			movementComponent.ResetSpeed();
		}
	}
}
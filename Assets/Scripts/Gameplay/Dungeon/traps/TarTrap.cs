using UnityEngine;

public class TarTrap : Trap
{
	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		CharacterMovement movementComponent;
		if (other.TryGetComponent(out movementComponent))
		{
			//TODO slow a heroes movement
			//movementComponent.SlowMovement() // Adjust damage as needed
		}
	}
}
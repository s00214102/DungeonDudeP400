using UnityEngine;

public class SpikeTrap : Trap
{
	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		Health healthComponent;
		if (other.TryGetComponent(out healthComponent))
		{
			healthComponent.TakeDamage(damage); // Adjust damage as needed
		}
	}
}
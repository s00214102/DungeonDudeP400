using System.Collections;
using UnityEngine;

public class SpikeTrap : Trap
{
	protected override void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Hero"))
		{
			Health healthComponent;
			if (other.TryGetComponent(out healthComponent))
			{
				healthComponent.TakeDamage(damage); // Adjust damage as needed
				Disarm();
			}
		}
	}
	protected override void OnTriggerExit(Collider other)
	{

	}
}
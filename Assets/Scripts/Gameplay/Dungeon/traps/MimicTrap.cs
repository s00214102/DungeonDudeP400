using UnityEngine;

public class MimicTrap : Trap
{
	//TODO when a hero uses the Treasure component of the mimic they take damage
	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
	}
}
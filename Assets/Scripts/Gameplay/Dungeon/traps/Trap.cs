using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class Trap : MonoBehaviour
{
	public int damage = 10;
	public int uses = 1; // Number of uses before trap is destroyed
	[Range(0f, 3f)] // Define the range for the slider
	public int disarmDifficulty = 1; // Difficulty level to disarm the trap

	protected List<GameObject> targets = new List<GameObject>(); // List of targets in collision

	// Event to signal when a new target is detected
	public delegate void TargetDetectedHandler(GameObject target);
	public event TargetDetectedHandler OnTargetDetected;

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Hero"))
		{
			if (!targets.Contains(other.gameObject))
			{
				targets.Add(other.gameObject);
				OnTargetDetected?.Invoke(other.gameObject);
			}
		}
	}

	protected virtual void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Hero"))
		{
			targets.Remove(other.gameObject);
		}
	}
	//TODO a hero can disarm a trap
	// most heroes will just walk over the trap and take damage
	// a hero with the ??? trait will prioritise the Disarm goal/action
	// the trap disarm difficulty make it take longer
	// once a hero starts to disarm it just takes time
	// there is a chance they fail and hurt themselves
	public virtual void Disarm()
	{
		uses--;
		if (uses <= 0)
		{
			Destroy(gameObject);
		}
	}
}
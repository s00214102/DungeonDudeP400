using UnityEngine;

public class MimicTrap : Trap
{
	//TODO when a hero uses the Treasure component of the mimic they take damage
	Treasure treasure;

	private void Awake()
	{
		treasure = GetComponentInChildren<Treasure>();
	}
	private void Start()
	{
		if (treasure != null)
			treasure.OnLooted.AddListener(DealDamage);
		else
			Debug.LogWarning("MimicTrap missing Treasure component");
	}
	private void DealDamage()
	{
		// deal damage to all targets next to the mimic
		foreach (var target in targets)
		{
			Health healthComponent;
			if (target.TryGetComponent(out healthComponent))
			{
				healthComponent.TakeDamage(damage);
			}
		}
		if (!treasure.HasLoot())
			DestroyTrap();
	}
}
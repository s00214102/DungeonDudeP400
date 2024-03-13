using System.Collections.Generic;
using UnityEngine;

// for now the hero heals via the Angel_Healer, which is manually cached in the inspector
// in reality they should search for any healing method and remember it
// then choose what healing option to use when they need to heal
// they may have seen an angel healer, or know of another teammate that can heal, or maybe they have a potion
public class Action_Heal : Action_Base
{
	List<System.Type> SupportedGoals = new List<System.Type> { typeof(Goal_Heal) };
	public override List<System.Type> GetSupportedGoals()
	{
		return SupportedGoals;
	}

	public override float GetCost()
	{
		return 0f;
	}

	public override void OnActivated(Goal_Base _linkedGoal)
	{
		base.OnActivated(_linkedGoal);
		// Additional activation code here
		goap_debug.ChangeActionImage(2);
		movement.MoveTo(data.Angel.position);
		movement.DestinationReached.AddListener(Pray);
	}
	private void Pray()
	{
		// when the hero reaches the angel they will pray to regain their HP
		goap_debug.ChangeActionImage(6);
		movement.StopMoving();
		isPraying = true;
	}
	public override void OnDeactived()
	{
		base.OnDeactived();
		// Additional deactivation code here
		movement.StopMoving();
		movement.DestinationReached.RemoveListener(Pray);
		isPraying = false;
		prayTimer = 0;
	}
	private bool isPraying = false;
	private float prayTimer = 0;
	private float prayTimeToCount = 5;
	public override void OnTick()
	{
		if (isPraying)
		{
			prayTimer += (1.0f * Time.deltaTime);
			if (prayTimer >= prayTimeToCount)
			{
				Angel_Healer angelHealer = data.Angel.GetComponent<Angel_Healer>();
				if (angelHealer.CanHeal())
				{
					health.HealToFull();
					angelHealer.UseCharge();
				}
				isPraying = false;
			}
		}
	}
}
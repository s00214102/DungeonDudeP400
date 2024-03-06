using System.Collections.Generic;
using UnityEngine;

public class Action_Base : MonoBehaviour
{
	protected CharacterMovement movement;
	protected EntityProximityDetectionBT detection;
	protected Goal_Base LinkedGoal;

	private void Awake()
	{
		movement = GetComponent<CharacterMovement>();
		detection = GetComponent<EntityProximityDetectionBT>();
	}
	public virtual System.Type[] GetSupportedGoals()
	{
		return null;
	}
	public virtual float GetCost()
	{
		return 0f;
	}
	public virtual void OnActivated()
	{

	}
	public virtual void OnDeactived()
	{

	}
	public virtual void OnTick()
	{

	}
}
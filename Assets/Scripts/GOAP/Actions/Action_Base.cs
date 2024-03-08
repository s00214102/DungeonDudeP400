using System.Collections.Generic;
using UnityEngine;

public class Action_Base : MonoBehaviour
{
	protected CharacterMovement movement;
	protected EntityProximityDetectionBT detection;
	protected GOAP_Hero_Data data;
	protected GOAP_Debug goap_debug;
	protected Goal_Base LinkedGoal;

	private void Awake()
	{
		movement = GetComponent<CharacterMovement>();
		detection = GetComponent<EntityProximityDetectionBT>();
		data = GetComponent<GOAP_Hero_Data>();
		goap_debug = GetComponent<GOAP_Debug>();
	}
	public virtual List<System.Type> GetSupportedGoals()
	{
		return null;
	}
	public virtual float GetCost()
	{
		return 0f;
	}
	public virtual void OnActivated(Goal_Base _linkedGoal)
	{
		LinkedGoal = _linkedGoal;
	}
	public virtual void OnDeactived()
	{
		LinkedGoal = null;
	}
	public virtual void OnTick()
	{

	}
}
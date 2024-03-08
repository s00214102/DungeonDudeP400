using System;
using UnityEngine;

public class GOAP_Planner : MonoBehaviour
{
	Goal_Base[] Goals;
	Action_Base[] Actions;

	[SerializeField] Goal_Base ActiveGoal;
	[SerializeField] Action_Base ActiveAction;

	private void Awake()
	{
		Goals = GetComponents<Goal_Base>();
		Actions = GetComponents<Action_Base>();
	}
	private void Update()
	{
		Goal_Base bestGoal = null;
		Action_Base bestAction = null;

		foreach (var goal in Goals)
		{
			goal.OnTickGoal();

			// can it run?
			if (!goal.CanRun())
				continue;

			// is it a worse priority?
			if (!(bestGoal == null || goal.OnCalculatePriority() > bestGoal.OnCalculatePriority()))
				continue;

			// find the best cost action
			Action_Base candidateAction = null;
			foreach (var action in Actions)
			{
				if (!action.GetSupportedGoals().Contains(goal.GetType()))
					continue;

				// found a suitable action
				if (candidateAction == null || action.GetCost() < candidateAction.GetCost())
					candidateAction = action;
			}
			// did we find an action?
			if (candidateAction != null)
			{
				bestGoal = goal;
				bestAction = candidateAction;
			}
		}

		// if no current goal
		if (ActiveGoal == null)
		{
			ActiveGoal = bestGoal;
			ActiveAction = bestAction;

			if (ActiveGoal != null)
				ActiveGoal.OnGoalActivated(ActiveAction);
			if (ActiveAction != null)
				ActiveAction.OnActivated(ActiveGoal);

		}
		// no change in goal?
		else if (ActiveGoal == bestGoal)
		{
			// action changed?
			if (ActiveAction != bestAction)
			{
				ActiveAction.OnDeactived();
				ActiveAction = bestAction;
				ActiveAction.OnActivated(ActiveGoal);
			}

		} // new goal or no valid goal?
		else if (ActiveGoal != bestGoal)
		{
			ActiveGoal.OnGoalDeactivated();
			ActiveAction.OnDeactived();

			ActiveGoal = bestGoal;
			ActiveAction = bestAction;

			if (ActiveGoal != null)
				ActiveGoal.OnGoalActivated(ActiveAction);
			if (ActiveAction != null)
				ActiveAction.OnActivated(ActiveGoal);
		}
		// finally tick the active action
		if (ActiveAction != null)
			ActiveAction.OnTick();
	}
}
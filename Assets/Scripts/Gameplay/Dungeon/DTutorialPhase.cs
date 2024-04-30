using UnityEngine;
using UnityEngine.Events;

public class DTutorialPhase : MonoBehaviour
{
	[HideInInspector] public GameplayController gameplayController;

	public void Begin()
	{
		Helper.SetChildrenActive(this.gameObject, true);
	}
}
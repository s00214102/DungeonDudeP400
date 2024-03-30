using UnityEngine;

public class DungeonLosePhase : MonoBehaviour
{
	public GameplayController gameplayController;

	public void Begin()
	{
		Helper.SetChildrenActive(this.gameObject, true);
	}
}
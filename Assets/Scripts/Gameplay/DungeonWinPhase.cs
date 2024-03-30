using UnityEngine;

public class DungeonWinPhase : MonoBehaviour
{
	public GameplayController gameplayController;

	public void Begin()
	{
		Helper.SetChildrenActive(this.gameObject, true);
	}
}
using UnityEngine;

public class DungeonPlayPhase : MonoBehaviour
{
	[SerializeField] private int heroesToSpawn = 0;
	[SerializeField] private float spawnFrequency = 1; // how often is a new hero spawned
	[SerializeField] private float frequencyVariation = 1; // add some variance to the spawn frequency
	[SerializeField] private GameObject heroPrefab; // the base hero prefab to spawn

	public GameplayController gameplayController;

	public void Begin()
	{
		Helper.SetChildrenActive(this.gameObject, true);
	}
}
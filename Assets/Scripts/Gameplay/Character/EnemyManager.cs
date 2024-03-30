using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	[SerializeField] private List<EnemyController> enemies = new();

	public void AddEnemy(GameObject enemy)
	{
		enemies.Add(enemy.GetComponent<EnemyController>());
	}
	void Update()
	{
		// simplest implementation, simply call each update function in one go
		foreach (var enemy in enemies)
		{
			enemy.ManagerUpdate();
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay", menuName = "Data/Gameplay")]
public class GameplayData : ScriptableObject
{
	public float TimeToCount;
	public int HeroesToSpawn;
	public float HeroSpawnInterval;
	public float HeroSpawnIntervalVariation;
	public int StartingEnergy;
	public int EnergyRegenPerSecond;
}

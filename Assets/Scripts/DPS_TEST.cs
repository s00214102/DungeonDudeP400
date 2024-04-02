using UnityEngine;

public class DPS_TEST : MonoBehaviour
{
	public float damagePerSecond; // DPS value
	public float attackRate; // Attack rate (in seconds per attack)
	public float damageValue; // Damage value per attack

	// Method to calculate attack rate and damage value based on DPS
	public void CalculateValues()
	{
		attackRate = 1f / damagePerSecond; // Calculate attack rate
		damageValue = damagePerSecond * attackRate; // Calculate damage value per attack
	}
}
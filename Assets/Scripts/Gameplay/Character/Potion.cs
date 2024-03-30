using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Potion : Item
{
	public int PotionStrength; // Strength of the potion's effect

	// Constructor to initialize the potion
	public Potion(string itemName, int count, int value, Image image, int strength)
	{
		ItemName = itemName;
		Count = count;
		Value = value;
		Image = image;
		PotionStrength = strength;
	}
}

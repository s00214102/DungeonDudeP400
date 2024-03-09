using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum HeroArchetype
{
	Fearless,
	Plunderer,
	Guardian,
	Scholar,
	Fool,
	Explorer,
	Oathsworn,
	Berserker,
	LoneWolf,
	Strategist,
	Coward,
}

[CreateAssetMenu(fileName = "Hero Traits", menuName = "Data/Hero")]
public class HeroTraits : ScriptableObject
{
	public int Bravery;     // how easy do they scare?
	public int Greed;       // loot prioritisation
	public int Altruism;    // do they help their team mates?
	public int Wisdom;      // how much can they remember?
	public int Curiosity;   // do they explore?
	public int Loyalty;     // do they stick with their fellow heroes?
	public int Aggression;  // attack prioritisation
	public int Caution;     // do they move cautiously checking for traps?
	public HeroArchetype HeroArchetype;
}

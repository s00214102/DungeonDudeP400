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

[CreateAssetMenu(fileName = "Hero Traits", menuName = "Data/Hero Traits")]
public class HeroTraits : ScriptableObject
{
	[Range(0, 3)] public int Bravery;     // how easy do they scare?
	[Range(0, 3)] public int Greed;       // loot prioritisation
	[Range(0, 3)] public int Altruism;    // do they help their team mates?
	[Range(0, 3)] public int Wisdom;      // how much can they remember?
	[Range(0, 3)] public int Curiosity;   // do they explore?
	[Range(0, 3)] public int Loyalty;     // do they stick with their fellow heroes?
	[Range(0, 3)] public int Aggression;  // attack prioritisation
	[Range(0, 3)] public int Caution;     // do they move cautiously checking for traps?
	public HeroArchetype HeroArchetype;
}

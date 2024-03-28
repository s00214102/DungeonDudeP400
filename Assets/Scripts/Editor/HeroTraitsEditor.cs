using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HeroTraits))]
public class HeroTraitsEditor : Editor
{
	public override void OnInspectorGUI()
	{
		HeroTraits traits = (HeroTraits)target;

		// Draw the default inspector GUI
		DrawDefaultInspector();

		// Validate the trait values
		ValidateTraitValues(traits);
	}

	private void ValidateTraitValues(HeroTraits traits)
	{
		// Array to store the trait values
		int[] traitValues = new int[]
		{
			traits.Bravery,
			traits.Greed,
			traits.Altruism,
			traits.Wisdom,
			traits.Curiosity,
			traits.Loyalty,
			traits.Aggression,
			traits.Caution
		};

		// Count the number of traits with value 3
		int three_count = 0;
		foreach (int value in traitValues)
		{
			if (value == 3)
			{
				three_count++;
			}
		}
		// If more than one trait has value 3, display a warning
		if (three_count > 1)
		{
			EditorGUILayout.HelpBox("Only one trait can have a value of 3 at a time.", MessageType.Warning);
		}

		// Count the number of traits with value 2
		int two_count = 0;
		foreach (int value in traitValues)
		{
			if (value == 2)
			{
				two_count++;
			}
		}
		// If more than two traits have a value of 2, display a warning
		if (two_count > 2)
		{
			EditorGUILayout.HelpBox("Only two traits can have a value of 2 at a time.", MessageType.Warning);
		}

		// Count the number of traits with value 1
		int one_count = 0;
		foreach (int value in traitValues)
		{
			if (value == 1)
			{
				one_count++;
			}
		}
		// If more than two traits have a value of 2, display a warning
		if (one_count > 3)
		{
			EditorGUILayout.HelpBox("Only three traits can have a value of 1 at a time.", MessageType.Warning);
		}
	}
}

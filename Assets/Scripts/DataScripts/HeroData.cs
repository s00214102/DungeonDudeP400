using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum HeroClass
{
    Knight,
    Archer,
    Rogue,
    Priest
}

[CreateAssetMenu(fileName = "Hero Stats", menuName = "Data/Hero")]
public class HeroData : ScriptableObject
{
    public int HitPoints;
    public int Damage;
    public float AttackRange;
    public float AttackRate;
    public float InteractRange; // how close to get to something when interacting with it
    public HeroClass HeroClass;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Data/Character")]
public class CharacterData : ScriptableObject
{
    public int HitPoints;
    public int Damage;        
}

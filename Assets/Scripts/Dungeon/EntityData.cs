using UnityEngine;
// Data about monsters and traps
public class EntityData : MonoBehaviour {
    // fear aura radius
    [SerializeField] private float fearRadius;
    public float FearRadius {get => fearRadius;}
    // fear damage value
    [SerializeField] private float fearDamage;
    public float FearDamage {get=>fearDamage;}
    [SerializeField] private float fearFrequency;
    public float FearFrequency {get=>fearFrequency;}
}
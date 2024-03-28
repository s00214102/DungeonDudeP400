using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A generic parent component which holds references to other generic components on an entity
// such as Health, EntityProximitDetection, etc.
// If you want to store a reference of an entity, you can store this component
[RequireComponent(typeof(EntityData))]
public class Entity : MonoBehaviour
{
    [HideInInspector] public Health Health;

    private void Awake()
    {
        Health = GetComponent<Health>();
    }
}

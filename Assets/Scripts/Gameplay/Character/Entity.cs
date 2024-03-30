using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A generic parent component which holds references to other generic components on an entity
// such as Health, EntityProximitDetection, etc.
// If you want to store a reference of an entity, you can store this component
public class Entity : MonoBehaviour
{
    [HideInInspector] public Health Health;
    public bool isDestroyed = false; // 

    private void Awake()
    {
        Health = GetComponent<Health>();
    }
    public void DisableEntity()
    {
        //Get all components attached to the GameObject
        Component[] components = gameObject.GetComponents<Component>();

        // Disable each component (except for Transform which should always be enabled)
        foreach (var component in components)
        {
            if (component.GetType() != typeof(Transform) && component.GetType() != typeof(Entity))
            {
                // Disable the component
                // Ensure it's a Behaviour before disabling
                if (component is Behaviour)
                {
                    MonoBehaviour monoBehaviour = component as MonoBehaviour;
                    if (monoBehaviour != null)
                        monoBehaviour.CancelInvoke();

                    (component as Behaviour).enabled = false;
                }
                if (component is Renderer)
                {
                    (component as Renderer).enabled = false;
                }
                if (component is Collider)
                {
                    (component as Collider).enabled = false;
                }
            }
        }
        gameObject.SetActive(false);
        isDestroyed = true;
    }
}

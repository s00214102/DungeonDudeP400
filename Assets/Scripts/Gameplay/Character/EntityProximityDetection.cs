using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Basic form of proximity detection which searches for entities by tag
public class EntityProximityDetection : MonoBehaviour
{
    [SerializeField] string target = "name of target"; // name of the tag on the entity you want to search for
    public float range = 1; // the range of detection
    [SerializeField] float frequency = 0.5f; // how long to wait between searches
    [SerializeField] float startDelay = 0; // how long to wait before starting the search
    [SerializeField] float closestTargetStartDelay = 0; // how long to wait before starting the search
    [SerializeField] float closestTargetFrequency = 0.5f; // wait between closest target calculation
    [SerializeField] private List<Entity> entities = new List<Entity>();
    public List<Entity> Entities { get => entities; }
    public Entity closestTarget; // closest 

    public void Start()
    {
        InvokeRepeating("SearchForTargets", startDelay, frequency);
        InvokeRepeating("FindClosestTarget", closestTargetStartDelay, closestTargetFrequency);
    }

    private void SearchForTargets()
    {
        var hits = Physics.OverlapSphere(transform.position, range);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.CompareTag(target))
                {
                    Entity entity;
                    if (hit.TryGetComponent<Entity>(out entity))
                    {
                        if (!entities.Contains(entity))
                        {
                            entities.Add(entity);
                            Health entityHealth = entity.gameObject.GetComponent<Health>();
                            //entityHealth.OnDied.AddListener(delegate{ForgetEntity(entity);});
                        }
                    }
                    else
                        Debug.Log($"{hit.name} missing Entity component?");
                }
            }
        }
    }
    // private void ForgetEntity(Entity entity){

    //     if(entity==closestTarget)
    //         closestTarget=null;

    //     Entities.Remove(entity);
    // }
    private void FindClosestTarget()
    {
        // clean up the list first (remove dead entities)
        closestTarget=null;
        for (int i = entities.Count - 1; i >= 0; i--)
        {
            GameObject obj = entities[i].gameObject;
            if (obj.GetComponent<Health>().isDead)
            {
                entities.RemoveAt(i);
                //Destroy(obj);
            }
        }

        if (entities == null || entities.Count <= 0)
            return;

        if (entities.Count == 1)
            closestTarget = entities[0];

        float minDistance = Mathf.Infinity;
        foreach (var target in entities)
        {
            float dist = Vector3.Distance(target.transform.position, transform.position);
            if (dist < minDistance && !target.Health.isDead)
            {
                minDistance = dist;
                closestTarget = target;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

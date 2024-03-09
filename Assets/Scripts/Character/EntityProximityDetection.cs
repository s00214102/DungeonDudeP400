using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

// Basic form of proximity detection which searches for entities by tag
public class EntityProximityDetection : MonoBehaviour
{
    [SerializeField] string target = "name of target"; // name of the tag on the entity you want to search for
    [SerializeField] float range = 1; // the range of detection
    [SerializeField] float frequency = 1; // how long to wait between searches
    [SerializeField] float startDelay = 0; // how long to wait before starting the search
    [SerializeField] float closestTargetStartDelay = 0; // how long to wait before starting the search
    [SerializeField] float closestTargetFrequency = 1; // wait between closest target calculation
    [SerializeField] private List<Entity> Entities = new List<Entity>();
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
                        if (!Entities.Contains(entity))
                        {
                            Entities.Add(entity);
                        }
                    }
                    else
                        Debug.Log($"{hit.name} missing Entity component?");
                }
            }
        }
    }

    private void FindClosestTarget()
    {
        //Debug.Log("Finding closest target.");
        if (Entities == null || Entities.Count <= 0)
            return;

        if (Entities.Count == 1)
            closestTarget = Entities[0];

        float minDistance = Mathf.Infinity;
        foreach (var target in Entities)
        {
            // clean up
            if (target.Health.isDead)
            {
                //if (target == closestTarget)
                closestTarget = null;
                Entities.Remove(target);
                return;
            }
            else
            {
                float dist = Vector3.Distance(target.transform.position, transform.position);
                if (dist < minDistance && !target.Health.isDead)
                {
                    minDistance = dist;
                    closestTarget = target;
                }
            }
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

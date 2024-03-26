using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

// Proximity detection specific to heroes
// this will be replaced by a more advanced detection system later
// the hero uses one overlap sphere cast to look for multiple things
// Enemies which are Entities, and other things like loot, healing, which are stored as positions
// this component should work with the HeroKnowledge component
[RequireComponent(typeof(HeroKnowledge))]
public class HeroProximityDetection : MonoBehaviour
{
    [SerializeField] float range = 1; // the range of detection
    [SerializeField] float frequency = 1; // how long to wait between searches
    [SerializeField] float startDelay = 0; // how long to wait before starting the search
    [SerializeField] float closestTargetStartDelay = 0; // how long to wait before starting the search
    [SerializeField] float closestTargetFrequency = 1; // wait between closest target calculation

    private HeroKnowledge knowledge;
    private void Awake()
    {
        knowledge = GetComponent<HeroKnowledge>();
    }
    public void Start()
    {
        InvokeRepeating("SearchForTargets", startDelay, frequency);
        InvokeRepeating("FindClosestEntity", closestTargetStartDelay, closestTargetFrequency);
    }
    // note: tags are looked for first as its more performant than checking for a component
    private void SearchForTargets()
    {
        var hits = Physics.OverlapSphere(transform.position, range);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                // Enemy search
                if (hit.CompareTag("Enemy"))
                {
                    Entity entity;
                    if (hit.TryGetComponent<Entity>(out entity))
                    {
                        if (!knowledge.Entities.Contains(entity))
                        {
                            knowledge.Entities.Add(entity);
                        }
                    }
                    else
                        Debug.Log($"{hit.name} missing Entity component?");
                }
                // Treasure search
                if (hit.CompareTag("Treasure"))
                {
                    knowledge.RememberItem("Treasure", hit.gameObject, true);
                }
                // Angel search
                if (hit.CompareTag("Angel"))
                {
                    knowledge.RememberItem("Angel", hit.gameObject, true);
                }
            }
        }
    }

    private void FindClosestEntity()
    {
        //Debug.Log("Finding closest target.");
        if (knowledge.Entities == null || knowledge.Entities.Count <= 0)
            return;

        if (knowledge.Entities.Count == 1)
            knowledge.closestTarget = knowledge.Entities[0];

        float minDistance = Mathf.Infinity;
        foreach (var target in knowledge.Entities)
        {
            // clean up
            if (target.Health.isDead)
            {
                //if (target == closestTarget)
                knowledge.closestTarget = null;
                knowledge.Entities.Remove(target);
                return;
            }
            else
            {
                float dist = Vector3.Distance(target.transform.position, transform.position);
                if (dist < minDistance && !target.Health.isDead)
                {
                    minDistance = dist;
                    knowledge.closestTarget = target;
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

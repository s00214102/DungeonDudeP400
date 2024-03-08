using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

// this EntityProximityDection system is particular to the BT setup
// it start scanning at the beginning and does stop
// it adds and removes entities from the list as they come and go
public class EntityProximityDetectionBT : MonoBehaviour
{
    [SerializeField] string target; // name of the tag on the entity you want to search for
    [SerializeField] float range; // the range of detection
    [SerializeField] float frequency; // how long to wait between searches
    [SerializeField] float startDelay; // how long to wait before starting the search
    //[SerializeField] bool stopSearch = false;
    //bool searching = false;
    [SerializeField] float closestTargetStartDelay;
    [SerializeField] float closestTargetFrequency; // wait between closest target calculation
    //public UnityEvent<Transform> TargetFound;
    [SerializeField] private List<Entity> Targets = new List<Entity>();
    public Entity closestTarget; // closest 

    public void Start()
    {
        //StartCoroutine(SearchForTargets());
        //StartCoroutine(FindClosestTarget());
        InvokeRepeating("SearchForTargets", startDelay, frequency);
        InvokeRepeating("FindClosestTarget", closestTargetStartDelay, closestTargetFrequency);
    }

    private void FindClosestTarget()
    {
        //Debug.Log("Finding closest target.");
        if (Targets == null || Targets.Count <= 0)
            return;

        if (Targets.Count == 1)
            closestTarget = Targets[0];

        float minDistance = Mathf.Infinity;
        foreach (var target in Targets)
        {
            // clean up
            if (target.Health.isDead)
            {
                //if (target == closestTarget)
                closestTarget = null;
                Targets.Remove(target);
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

    private void SearchForTargets()
    {
        //Debug.Log("Searching for targets.");

        var hits = Physics.OverlapSphere(transform.position, range);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.CompareTag(target))
                {
                    //Debug.Log($"{target} found");
                    //TargetFound.Invoke(hit.gameObject.transform);
                    Entity entity = hit.GetComponent<Entity>();
                    if (!Targets.Contains(entity))
                        Targets.Add(entity);
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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class EntityProximityDetection : MonoBehaviour
{
    [SerializeField] string target; // name of the tag on the entity you want to search for
    [SerializeField] float range; // the range of detection
    [SerializeField] float frequency; // how long to wait between searches
    [SerializeField] float startDelay; // how long to wait before starting the search
    [SerializeField] bool stopSearch = false;
    bool searching = false;

    public UnityEvent<Transform> TargetFound;
    public Transform Target;

    public void StartSearch()
    {
        StartCoroutine(SearchForTargets());

    }
    private IEnumerator SearchForTargets()
    {
        //Debug.Log("Search started");
        yield return new WaitForSeconds(startDelay);

        while (!stopSearch)
        {
            var hits = Physics.OverlapSphere(transform.position, range);
            if(hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.CompareTag(target))
                    {
                        //Debug.Log($"{target} found");
                        stopSearch = true;
                        TargetFound.Invoke(hit.gameObject.transform);
                        Target = hit.gameObject.transform;
                    }
                }
            }

            yield return new WaitForSeconds(frequency);
        }
        //StopCoroutine(SearchForTargets());
        stopSearch = false;
        yield break;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,range);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// component used to manage all BT_Hero entities, calls their updates
// If using this component, make sure to not call tree.Tick() in the BT_Hero_Controller
public class BT_Hero_Manager : MonoBehaviour
{
    private List<BT_Hero_Controller> Heroes = new();
    int tracker = 0;
    int batchsize = 10;

    private void Awake()
    {
        GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");

        foreach (var hero in heroes)
        {
            Heroes.Add(hero.GetComponent<BT_Hero_Controller>());
        }
    }
    void Update()
    {
        // simplest implementation, simply call each update function in one go
        foreach (var hero in Heroes)
        {
            float startTime = Time.time;
            Debug.Log("start compute time: " + startTime);
            hero.DirectorUpdate();
            float endTime = Time.time;
            Debug.Log("end compute time: " + endTime);
            //float totalTime = endTime - startTime;

            //Debug.Log("Total compute time: "+totalTime);
        }
    }
}

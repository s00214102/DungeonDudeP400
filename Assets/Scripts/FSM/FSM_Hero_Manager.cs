using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Hero_Manager : MonoBehaviour
{
    [SerializeField]private List<HeroController> Heroes = new();

    private void Awake()
    {
        //GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");

        //foreach (var hero in heroes)
        //{
        //    Heroes.Add(hero.GetComponent<HeroController>());
        //}
    }
    public void AddHero(GameObject hero)
    {
        Heroes.Add(hero.GetComponent<HeroController>());
    }
    void Update()
    {
        // simplest implementation, simply call each update function in one go
        foreach (var hero in Heroes)
        {
            hero.ManagerUpdate();
        }
    }
}

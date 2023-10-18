using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    Health Health;

    private void Start()
    {
        Health = GetComponent<Health>();
        //Health.SetMaxHealth(20);
    }
}

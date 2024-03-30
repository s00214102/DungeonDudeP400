using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    Health Health;
    private EnemyManager _manager;
    private bool _managed = false;

    private void Awake()
    {
        _manager = GetComponentInParent<EnemyManager>();
        if (_manager != null)
        {
            _managed = true;
            _manager.AddEnemy(this.gameObject);
        }
        else
            _managed = false;
    }
    private void Start()
    {
        Health = GetComponent<Health>();
        //Health.SetMaxHealth(20);
    }
    public void ManagerUpdate()
    {
        if (!_managed)
            return;
    }
    private void Update()
    {
        if (_managed)
            return;

    }
}

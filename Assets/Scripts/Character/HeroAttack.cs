using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HeroAttack : MonoBehaviour
{
    private HeroData _data;
    private Health enemyHealth;
    [HideInInspector] public bool attacking = false;
    private void Awake()
    {
        _data = GetComponent<BT_Hero_Controller>().HeroData;
    }
    public void StartAttacking(Transform enemy)
    {
        Debug.Log("HeroAttack - StartAttacking");

        enemyHealth = enemy.GetComponent<Health>();
        // if we have the enemy health component, and its not destroyed, and were not already attacking
        // then start attacking
        if (enemyHealth != null && !enemyHealth.isDead && !attacking)
        {
            //enemyHealth.EntityDied.AddListener()
            Debug.Log("HeroAttack - InvokeRepeating");
            attacking = true;
            InvokeRepeating("AttackTarget", 0, _data.AttackRate);
        }
        else
            Debug.Log("Health component could not be retreived.");
    }
    public void StopAttacking()
    {
        attacking = false;
        CancelInvoke("AttackTarget");
    }
    private void AttackTarget()
    {
        Debug.Log("HeroAttack - dealing damage to enemy");
        if (enemyHealth != null)
            enemyHealth.TakeDamage(_data.Damage);
        else
        {
            Debug.Log("HeroAttack - Enemy not found, nothing to attack.");
            StopAttacking();
        }
    }
}

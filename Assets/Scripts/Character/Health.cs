using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // set max health from the entities contoller class !!!
    [SerializeField]private int MaxHealth;
    private int CurrentHealth;
    private GameObject dpuObject;

    private void Awake()
    {
        dpuObject = Resources.Load("DamagePopUp") as GameObject;
        if (dpuObject == null)
        {
            Debug.LogWarning("Health component could not find the DamagePopUp file in resources folder.");
        }
    }
    private void Start()
    {
        CurrentHealth = MaxHealth;
    }
    public void SetMaxHealth(int value)
    {
        MaxHealth = value;
    }
    // subscribe to this to be notified when target dies
    public UnityEvent EntityDied;
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            EntityDied.Invoke();
            Destroy(this.gameObject);
        }

        // damage number stuff
        if (dpuObject == null)
        {
            Debug.LogWarning("Health component could not find the DamagePopUp file in resources folder.");
            return;
        }
        GameObject clone = Instantiate(dpuObject, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
        clone.GetComponent<DamagePopUp>().Setup(damage);
    }
}

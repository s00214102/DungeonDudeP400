using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // set max health from the entities contoller class !!!
    [SerializeField] private int maxHealth;
    public int MaxHealth => maxHealth;
    [SerializeField] private int currentHealth;
    public int CurrentHealth { get => currentHealth; }
    [SerializeField] private HealthBar healthBar;
    private GameObject dpuObject;
    public bool isDead = false; // tag as destroyed when hp reaches 0, these entities can be temporarily disable and then pernamentaly destroyed later

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
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }
    public void SetMaxHealth(int value)
    {
        maxHealth = value;
        healthBar.SetMaxHealth(maxHealth);
    }
    // subscribe to this to be notified when target dies
    public UnityEvent EntityDied;
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            healthBar.SetHealth(0);
            EntityDied.Invoke();
            isDead = true;

            gameObject.SetActive(false);
            //Destroy(this.gameObject);
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
    public void HealToFull()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }
}

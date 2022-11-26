using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] IntVariable maxHealth;

    [SerializeField] public int health;

    public UnityEvent<int> healthChanged;

    public void DecreaseHealth(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0;
        }

        healthChanged.Invoke(health);
    }

    public void IncreaseHealth(int amount)
    {
        health += amount;

        healthChanged.Invoke(health);
    }

    private void Awake()
    {
        if (maxHealth != null)
        {
            health = maxHealth.value;
        }
    }
}
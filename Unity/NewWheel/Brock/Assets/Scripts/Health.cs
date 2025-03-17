using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] public int maxHealth;

    [SerializeField] public int health;

    public UnityEvent<int> healthChanged;

    private int maxHealthValue = 10;

    public int applyChange(int amount)
    {
        int healthTemp = this.health + amount;
        if (healthTemp < 0)
        {
            healthTemp = 0;
        }
        else if (healthTemp > maxHealthValue)
        {
            healthTemp = maxHealthValue;
        }

        int changedAmount = healthTemp - this.health;
        this.health = healthTemp;
        healthChanged.Invoke(health);
        return changedAmount;
    }

    public int DecreaseHealth(int damage)
    {
        return Math.Abs(applyChange(-damage));
    }

    public int IncreaseHealth(int amount)
    {
        return applyChange(amount);
    }

    public void SetMaxHealth(int value)
    {
        this.maxHealth = value;
    }

    public bool IsFullHealth()
    {
        return health == maxHealth;
    }

    private void Awake()
    {
        maxHealthValue = maxHealth;
        health = maxHealth;
    }

    private void Start()
    {
        maxHealthValue = maxHealth;
        health = maxHealth;
    }
}
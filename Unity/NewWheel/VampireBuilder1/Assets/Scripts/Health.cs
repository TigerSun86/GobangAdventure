using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] public int health = 10;
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

}
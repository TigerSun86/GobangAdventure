using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public class Death : MonoBehaviour
{
    private Health health;
    [SerializeField] UnityEvent died;

    private void Awake()
    {
        health = GetComponent<Health>();
        health.healthChanged.AddListener(CheckDeath);
    }

    public void CheckDeath()
    {
        if (health.health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
        died.Invoke();
    }
}

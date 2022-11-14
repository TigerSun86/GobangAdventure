using System.Collections;
using System.Collections.Generic;
using Timers;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public class Death : MonoBehaviour
{
    [SerializeField] public UnityEvent<GameObject> died;

    [SerializeField] public float timeToLive = 0f;

    private void Awake()
    {
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.healthChanged.AddListener(CheckDeathByHealth);
        }

        if (!Mathf.Approximately(timeToLive, 0f))
        {
            TimersManager.SetTimer(this, timeToLive, Die);
        }
    }

    public void CheckDeathByHealth(int health)
    {
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        died.Invoke(gameObject);
        Destroy(gameObject);
    }
}

using Timers;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SelfDestroy))]
public class Death : MonoBehaviour
{
    [SerializeField] public UnityEvent<GameObject> deathEvent;

    [SerializeField] public float timeToLive = 0f;

    [SerializeField, Required] Health health;

    [SerializeField, AssignedInCode] SelfDestroy selfDestroy;

    private void Start()
    {
        health.healthChanged.AddListener(CheckDeathByHealth);

        selfDestroy = GetComponent<SelfDestroy>();

        if (!Mathf.Approximately(timeToLive, 0f))
        {
            TimersManager.SetTimer(this, timeToLive, Die);
        }
    }

    private void CheckDeathByHealth(int health)
    {
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        deathEvent.Invoke(gameObject);
        if (selfDestroy != null)
        {
            selfDestroy.Destroy();
        }
    }
}

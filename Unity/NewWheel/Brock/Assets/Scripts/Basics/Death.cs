using Timers;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SelfDestroy))]
public class Death : MonoBehaviour
{
    [SerializeField] public UnityEvent<GameObject> deathEvent;

    [SerializeField] public float timeToLive = 0f;

    [SerializeField, AssignedInCode] SelfDestroy selfDestroy;

    public void CheckDeathByHealth(int health)
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

    private void Start()
    {
        selfDestroy = GetComponent<SelfDestroy>();

        if (!Mathf.Approximately(timeToLive, 0f))
        {
            TimersManager.SetTimer(this, timeToLive, Die);
        }
    }
}

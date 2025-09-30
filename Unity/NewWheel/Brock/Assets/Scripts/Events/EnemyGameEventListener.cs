using UnityEngine;
using UnityEngine.Events;

public class EnemyGameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public EnemyGameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<Enemy> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(Enemy enemy)
    {
        Response.Invoke(enemy);
    }
}

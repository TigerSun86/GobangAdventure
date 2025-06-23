using UnityEngine;
using UnityEngine.Events;

public class BuffGameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public BuffGameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<GameObject, Buff> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(GameObject buffedGameObject, Buff buff)
    {
        Response.Invoke(buffedGameObject, buff);
    }
}

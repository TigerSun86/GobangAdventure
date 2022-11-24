using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Collider2DGameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public Collider2DGameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<Collider2D, GameObject> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(Collider2D other, GameObject bullet)
    {
        Response.Invoke(other, bullet);
    }
}

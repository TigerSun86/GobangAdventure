using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PositionsGameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public PositionsGameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<List<Vector3>> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(List<Vector3> positions)
    {
        Response.Invoke(positions);
    }
}

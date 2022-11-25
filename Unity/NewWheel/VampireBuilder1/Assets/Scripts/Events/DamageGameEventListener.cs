using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageGameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public DamageGameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<DamageData> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(DamageData damageData)
    {
        Response.Invoke(damageData);
    }
}

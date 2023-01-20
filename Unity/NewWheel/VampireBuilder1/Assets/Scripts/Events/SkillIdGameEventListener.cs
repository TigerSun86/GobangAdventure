using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillIdGameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public SkillIdGameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<SkillId> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(SkillId obj)
    {
        Response.Invoke(obj);
    }
}

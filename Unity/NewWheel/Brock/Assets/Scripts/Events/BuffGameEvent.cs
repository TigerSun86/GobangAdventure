using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/BuffGameEvent")]
public class BuffGameEvent : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<BuffGameEventListener> eventListeners =
        new List<BuffGameEventListener>();

    public void Raise(GameObject buffedGameObject, Buff buff)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
        {
            eventListeners[i].OnEventRaised(buffedGameObject, buff);
        }
    }

    public void RegisterListener(BuffGameEventListener listener)
    {
        if (!eventListeners.Contains(listener))
        {
            eventListeners.Add(listener);
        }
    }

    public void UnregisterListener(BuffGameEventListener listener)
    {
        if (eventListeners.Contains(listener))
        {
            eventListeners.Remove(listener);
        }
    }
}
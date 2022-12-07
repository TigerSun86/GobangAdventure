using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeEvent
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<UpgradeListener> eventListeners =
        new List<UpgradeListener>();

    public void Raise(int level)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
        {
            eventListeners[i].OnEventRaised(level);
        }
    }

    public void RegisterListener(UpgradeListener listener)
    {
        if (!eventListeners.Contains(listener))
        {
            eventListeners.Add(listener);
        }
    }

    public void UnregisterListener(UpgradeListener listener)
    {
        if (eventListeners.Contains(listener))
        {
            eventListeners.Remove(listener);
        }
    }
}

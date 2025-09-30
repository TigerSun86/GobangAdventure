using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/EnemyGameEvent")]
public class EnemyGameEvent : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<EnemyGameEventListener> eventListeners =
        new List<EnemyGameEventListener>();

    public void Raise(Enemy enemy)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
        {
            eventListeners[i].OnEventRaised(enemy);
        }
    }

    public void RegisterListener(EnemyGameEventListener listener)
    {
        if (!eventListeners.Contains(listener))
        {
            eventListeners.Add(listener);
        }
    }

    public void UnregisterListener(EnemyGameEventListener listener)
    {
        if (eventListeners.Contains(listener))
        {
            eventListeners.Remove(listener);
        }
    }
}
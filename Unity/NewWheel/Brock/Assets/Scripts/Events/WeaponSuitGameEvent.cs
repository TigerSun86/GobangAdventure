using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/WeaponSuitGameEvent")]
public class WeaponSuitGameEvent : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<WeaponSuitGameEventListener> eventListeners =
        new List<WeaponSuitGameEventListener>();

    public void Raise(WeaponSuit weaponSuit)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
        {
            eventListeners[i].OnEventRaised(weaponSuit);
        }
    }

    public void RegisterListener(WeaponSuitGameEventListener listener)
    {
        if (!eventListeners.Contains(listener))
        {
            eventListeners.Add(listener);
        }
    }

    public void UnregisterListener(WeaponSuitGameEventListener listener)
    {
        if (eventListeners.Contains(listener))
        {
            eventListeners.Remove(listener);
        }
    }
}
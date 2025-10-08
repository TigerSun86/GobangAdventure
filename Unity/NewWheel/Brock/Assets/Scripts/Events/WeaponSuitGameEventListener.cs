using UnityEngine;
using UnityEngine.Events;

public class WeaponSuitGameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public WeaponSuitGameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<WeaponSuit> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(WeaponSuit weaponSuit)
    {
        Response.Invoke(weaponSuit);
    }
}

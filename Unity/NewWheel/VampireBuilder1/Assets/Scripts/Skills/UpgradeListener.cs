using UnityEngine;
using UnityEngine.Events;

public class UpgradeListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public Skill skill;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<int> response;

    private void OnEnable()
    {
        skill.UpgradeEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        skill.UpgradeEvent.UnregisterListener(this);
    }

    public void OnEventRaised(int level)
    {
        response.Invoke(level);
    }
}

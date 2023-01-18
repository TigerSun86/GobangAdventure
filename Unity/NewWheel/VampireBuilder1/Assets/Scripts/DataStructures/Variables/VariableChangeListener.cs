using UnityEngine;
using UnityEngine.Events;

public class VariableChangeListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public VariableBase variable;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent response;

    private void OnEnable()
    {
        variable.variableChangeEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        variable.variableChangeEvent.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        response.Invoke();
    }
}

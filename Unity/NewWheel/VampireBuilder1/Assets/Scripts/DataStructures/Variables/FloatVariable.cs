using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class FloatVariable : VariableBase, ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public float defaultValue;

    public float value;

    public void SetValue(float value)
    {
        this.value = value;
        this.variableChangeEvent.Raise();
    }

    public void SetValue(FloatVariable value)
    {
        this.value = value.value;
        this.variableChangeEvent.Raise();
    }

    public void ApplyChange(float amount)
    {
        value += amount;
        this.variableChangeEvent.Raise();
    }

    public void ApplyChange(FloatVariable amount)
    {
        value += amount.value;
        this.variableChangeEvent.Raise();
    }

    public void Reset()
    {
        value = defaultValue;
        this.variableChangeEvent.Raise();
    }

    public void OnAfterDeserialize()
    {
        SceneManager.sceneLoaded += (a, b) => Reset();
        EditorApplication.playModeStateChanged += (a) => Reset();
    }

    public void OnBeforeSerialize()
    {
    }

    public override string ToString()
    {
        return value.ToString();
    }
}
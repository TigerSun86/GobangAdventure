using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Variables/IntVariable")]
public class IntVariable : VariableBase, ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public int defaultValue;

    public int value;

    public void SetValue(int value)
    {
        this.value = value;
        this.variableChangeEvent.Raise();
    }

    public void SetValue(IntVariable value)
    {
        this.value = value.value;
        this.variableChangeEvent.Raise();
    }

    public void ApplyChange(int amount)
    {
        value += amount;
        this.variableChangeEvent.Raise();
    }

    public void ApplyChange(IntVariable amount)
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
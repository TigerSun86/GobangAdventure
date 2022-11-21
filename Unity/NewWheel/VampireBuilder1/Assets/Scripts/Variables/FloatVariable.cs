using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class FloatVariable : ScriptableObject, ISerializationCallbackReceiver
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
    }

    public void SetValue(FloatVariable value)
    {
        this.value = value.value;
    }

    public void ApplyChange(float amount)
    {
        value += amount;
    }

    public void ApplyChange(FloatVariable amount)
    {
        value += amount.value;
    }

    public void Reset()
    {
        value = defaultValue;
    }

    public void OnAfterDeserialize()
    {
        SceneManager.sceneLoaded += (a, b) => Reset();
        EditorApplication.playModeStateChanged += (a) => Reset();
    }

    public void OnBeforeSerialize()
    {
    }
}
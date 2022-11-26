using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class IntVariable : ScriptableObject, ISerializationCallbackReceiver
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
    }

    public void SetValue(IntVariable value)
    {
        this.value = value.value;
    }

    public void ApplyChange(int amount)
    {
        value += amount;
    }

    public void ApplyChange(IntVariable amount)
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
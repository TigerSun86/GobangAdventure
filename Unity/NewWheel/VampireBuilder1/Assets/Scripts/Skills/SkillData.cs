using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillData : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] protected int level;

    public int GetLevel()
    {
        return level;
    }

    public int GetNextLevel()
    {
        return level + 1;
    }

    public virtual void LevelUp()
    {
        level++;
    }

    public virtual string GetName()
    {
        return this.GetType().Name;
    }

    public virtual string GetNextLevelDescription()
    {
        return "";
    }

    private void ResetLevel()
    {
        level = 0;
    }

    public void OnAfterDeserialize()
    {
        SceneManager.sceneLoaded += (a, b) => ResetLevel();
        EditorApplication.playModeStateChanged += (a) => ResetLevel();
    }

    public void OnBeforeSerialize()
    {
    }
}

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class Skill : ScriptableObject, ISerializationCallbackReceiver
{
    public string skillName;

    public int level;

    public int maxLevel;

    public List<SkillDependency> dependencies;

    public List<SkillDescription> skillDescriptions;

    [NonSerialized] public UpgradeEvent UpgradeEvent = new UpgradeEvent();

    public int GetNextLevel()
    {
        return level + 1;
    }

    public bool IsUpgradable()
    {
        if (maxLevel > 0 && level >= maxLevel)
        {
            return false;
        }

        foreach (SkillDependency dependency in dependencies)
        {
            if (!dependency.MeetRequirement())
            {
                return false;
            }
        }

        return true;
    }

    public void LevelUp()
    {
        level++;
        UpgradeEvent.Raise(level);
    }

    public string GetNextLevelDescription()
    {
        SkillDescription last = null;
        foreach (SkillDescription description in skillDescriptions)
        {
            if (description.level > this.GetNextLevel())
            {
                break;
            }

            last = description;
        }

        if (last != null)
        {
            return last.ToString();
        }

        Debug.LogError("Skill descriptions should not be empty");
        return null;
    }

    public void OnAfterDeserialize()
    {
        SceneManager.sceneLoaded += (a, b) => Reset();
        EditorApplication.playModeStateChanged += (a) => Reset();
    }

    public void OnBeforeSerialize()
    {
    }

    private void Reset()
    {
        level = 0;
    }
}

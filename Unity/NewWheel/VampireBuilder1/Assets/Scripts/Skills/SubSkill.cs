using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class SubSkill : ScriptableObject, ISerializationCallbackReceiver
{
    public int currentLevel;

    public GemType gemType;

    public List<SubSkillLevelInfo> levelInfos;

    private MainSkill mainSkill;

    public bool CanLevelUp()
    {
        return currentLevel < levelInfos.Max(l => l.level);
    }

    public void LevelUp()
    {
        int newLevel = currentLevel + 1;
        SubSkillLevelInfo levelInfo = levelInfos.FirstOrDefault(l => l.level == newLevel);
        if (levelInfo == null)
        {
            Debug.LogError($"Could not find the sub skill for level {newLevel}");
            return;
        }

        ApplyChange(mainSkill, levelInfo);

        currentLevel++;
    }

    public void SetMainSkill(MainSkill mainSkill)
    {
        this.mainSkill = mainSkill;
    }

    public void Reset()
    {
        currentLevel = 0;
    }

    public void OnAfterDeserialize()
    {
        SceneManager.sceneLoaded += (a, b) => Reset();
        EditorApplication.playModeStateChanged += (a) => Reset();
    }

    public void OnBeforeSerialize()
    {
    }

    private void ApplyChange(MainSkill mainSkill, SubSkillLevelInfo levelInfo)
    {
        float value = levelInfo.value;
        switch (levelInfo.attributeType)
        {
            case AttributeType.ATTACK:
                mainSkill.attack = value;
                break;
            default:
                throw new InvalidEnumArgumentException(
                    nameof(levelInfo.attributeType),
                    (int)levelInfo.attributeType,
                    typeof(AttributeType));
        }
    }
}
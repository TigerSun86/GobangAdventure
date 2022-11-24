using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    [SerializeField] private int level;

    [SerializeField] public int maxLevel;

    public int GetLevel()
    {
        return level;
    }

    public int GetNextLevel()
    {
        return level + 1;
    }

    public bool IsUpgradable()
    {
        return maxLevel <= 0 || level < maxLevel;
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
}

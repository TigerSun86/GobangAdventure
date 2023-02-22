using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SkillConfig
{
    public SkillId id;

    public string name;

    public string description;

    public List<AttributeTypeAndFloat> initialAttributes;

    public List<SkillDependency> dependencies;

    public List<SkillLevelConfig> levels;

    public AttributeTypeToFloatDictionary GetInitialAttributeDictionary()
    {
        AttributeTypeToFloatDictionary dict = new AttributeTypeToFloatDictionary();
        foreach (AttributeTypeAndFloat pair in initialAttributes)
        {
            dict[pair.attributeType] = pair.value;
        }

        return dict;
    }

    public int GetMaxLevel()
    {
        return levels.Max(l => l.level);
    }

    public SkillLevelConfig GetLevelConfig(int level)
    {
        return levels.First(l => l.level == level);
    }

    public string GetLevelDescription(int level)
    {
        return GetLevelConfig(level).description;
    }
}

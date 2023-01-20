using System;
using System.Collections.Generic;

[Serializable]
public class SkillConfig
{
    public SkillId id;

    public string name;

    public string description;

    public List<AttributeTypeAndFloat> initialAttributes;

    public List<SkillId> dependencies;

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
}

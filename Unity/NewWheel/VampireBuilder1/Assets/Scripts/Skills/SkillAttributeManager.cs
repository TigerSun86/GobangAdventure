using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOneOfEach/SkillAttributeManager")]
public class SkillAttributeManager : ScriptableObject
{
    [SerializeField] SkillIdToAttributesDictionary skillIdToAttributes;

    [SerializeField] List<SkillId> allowedSkills;

    public void Initialize(TbSkillConfig tbSkillConfig)
    {
        skillIdToAttributes.Clear();
        foreach (SkillConfig skillConfig in tbSkillConfig.GetAllSkillConfigs())
        {
            if (skillConfig.id != SkillId.COMMON && !allowedSkills.Contains(skillConfig.id))
            {
                continue;
            }

            SetAttributes(skillConfig.id, skillConfig.GetInitialAttributeDictionary());
            SetLevel(skillConfig.id, 0);
        }
    }

    public bool ContainsAttribute(SkillId skillId, AttributeType attributeType)
    {
        return skillIdToAttributes[skillId].ContainsKey(attributeType);
    }

    public AttributeTypeToFloatDictionary GetAttributes(SkillId skillId)
    {
        return skillIdToAttributes[skillId];
    }

    public float GetAttribute(SkillId skillId, AttributeType attributeType)
    {
        return GetAttributes(skillId)[attributeType];
    }

    public void SetAttributes(SkillId skillId, AttributeTypeToFloatDictionary attributes)
    {
        skillIdToAttributes[skillId] = attributes;
    }

    public void SetAttribute(SkillId skillId, AttributeType attributeType, float value)
    {
        GetAttributes(skillId)[attributeType] = value;
    }

    public IEnumerable<SkillId> GetAllSkills()
    {
        return skillIdToAttributes.Keys.Where(s => s != SkillId.COMMON);
    }

    public int GetLevel(SkillId skillId)
    {
        return (int)GetAttributes(skillId)[AttributeType.LEVEL];
    }

    public void SetLevel(SkillId skillId, int level)
    {
        GetAttributes(skillId)[AttributeType.LEVEL] = level;
    }

    public SkillBehaviorType? GetBehaviorType(SkillId skillId)
    {
        if (!ContainsAttribute(skillId, AttributeType.BEHAVIOR_TYPE))
        {
            return null;
        }

        int enumValue = (int)GetAttributes(skillId)[AttributeType.BEHAVIOR_TYPE];
        if (!Enum.IsDefined(typeof(SkillBehaviorType), enumValue))
        {
            throw new ArgumentException($"Value {enumValue} is not defined in enum type {nameof(SkillBehaviorType)}");
        }

        return (SkillBehaviorType)enumValue;
    }
}

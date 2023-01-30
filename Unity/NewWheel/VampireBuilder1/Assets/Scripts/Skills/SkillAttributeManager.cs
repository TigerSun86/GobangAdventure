using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOneOfEach/SkillAttributeManager")]
public class SkillAttributeManager : ScriptableObject
{
    [SerializeField] SkillIdToAttributesDictionary skillIdToAttributes;

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

    public void Clear()
    {
        skillIdToAttributes.Clear();
    }
}

using System;
using System.Collections.Generic;

[Serializable]
public class AttackData
{
    public float attack;

    public float criticalRate;

    public float criticalAmount;

    public AttackData(float attack, float criticalRate = 0, float criticalAmount = 2)
    {
        this.attack = attack;
        this.criticalRate = criticalRate;
        this.criticalAmount = criticalAmount;
    }

    public AttackData(SkillId skillId, SkillAttributeManager skillAttributeManager, float? attackOverride = null)
    {
        AttributeTypeToFloatDictionary commonAttributes = skillAttributeManager.GetAttributes(SkillId.COMMON);
        AttributeTypeToFloatDictionary skillAttributes = skillAttributeManager.GetAttributes(skillId);
        this.attack = attackOverride.HasValue ? attackOverride.Value : skillAttributes[AttributeType.ATTACK];
        this.attack *= commonAttributes[AttributeType.ATTACK];
        this.criticalRate = commonAttributes.GetValueOrDefault(AttributeType.CRITICAL_RATE, 0)
            + skillAttributes.GetValueOrDefault(AttributeType.CRITICAL_RATE, 0);
        this.criticalAmount = commonAttributes.GetValueOrDefault(AttributeType.CRITICAL_AMOUNT, 0)
            + skillAttributes.GetValueOrDefault(AttributeType.CRITICAL_AMOUNT, 0);
    }
}
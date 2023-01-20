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

    public AttackData(MainSkill mainSkill)
    {
        this.attack = mainSkill.attack;
        this.criticalRate = mainSkill.criticalRate;
        this.criticalAmount = mainSkill.criticalAmount;
    }

    public AttackData(AttributeTypeToFloatDictionary commonAttributes, AttributeTypeToFloatDictionary skillAttributes)
    {
        this.attack = commonAttributes[AttributeType.ATTACK] * skillAttributes[AttributeType.ATTACK];
        this.criticalRate = commonAttributes.GetValueOrDefault(AttributeType.CRITICAL_RATE, 0)
            + skillAttributes.GetValueOrDefault(AttributeType.CRITICAL_RATE, 0);
        this.criticalAmount = commonAttributes.GetValueOrDefault(AttributeType.CRITICAL_AMOUNT, 0)
            + skillAttributes.GetValueOrDefault(AttributeType.CRITICAL_AMOUNT, 0);
    }
}
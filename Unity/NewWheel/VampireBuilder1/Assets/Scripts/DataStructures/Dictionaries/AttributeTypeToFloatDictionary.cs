using System;
using System.Collections.Generic;

[Serializable]
public class AttributeTypeToFloatDictionary : SerializableDictionary<AttributeType, float>
{
    public static AttributeTypeToFloatDictionary CreateInstanceWithAllAttributes()
    {
        return new AttributeTypeToFloatDictionary()
        {
            {AttributeType.ATTACK, 1},
            {AttributeType.CRITICAL_RATE, 0.05f},
            {AttributeType.CRITICAL_AMOUNT, 2},
            {AttributeType.AREA, 1},
            {AttributeType.ATTACK_DECREASE, 1},
        };
    }

    public AttributeTypeToFloatDictionary()
    {
    }

    public AttributeTypeToFloatDictionary(AttributeTypeToFloatDictionary other)
    {
        foreach (KeyValuePair<AttributeType, float> kvp in other)
        {
            this[kvp.Key] = kvp.Value;
        }
    }
}
using System;
using System.Collections.Generic;

[Serializable]
public class AttributeTypeToFloatDictionary : SerializableDictionary<AttributeType, float>
{
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
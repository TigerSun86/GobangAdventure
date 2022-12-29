using System;

[Serializable]
public class SubSkillLevelInfo
{
    public int level;

    public string description;

    public AttributeType attributeType;

    public float value;

    public override string ToString()
    {
        return description;
    }
}
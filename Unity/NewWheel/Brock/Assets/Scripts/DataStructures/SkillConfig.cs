using System;

[Serializable]
public class SkillConfig
{
    public string skillName;

    public int level;

    public string description;

    public SkillType skillType;

    public float value;

    public float cdTime;

    public float actionTime;

    public float recoveryTime;

    public float projectileSpeed;

    public float range;

    public SkillTargetConfig skillTargetConfig;
}

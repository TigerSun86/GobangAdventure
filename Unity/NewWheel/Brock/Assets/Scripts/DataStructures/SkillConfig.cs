using System;

[Serializable]
public class SkillConfig
{
    public string skillName;

    public int level;

    public string description;

    public SkillType skillType;

    public SkillActivationType skillActivationType;

    public float value;

    public float cdTime;

    public float actionTime;

    public float recoveryTime;

    public float projectileSpeed;

    public float range;

    public SkillTargetConfig skillTargetConfig;

    public Buff buff1;

    public Buff buff2;
}

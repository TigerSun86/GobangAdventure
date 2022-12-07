using System;

[Serializable]
public class SkillDependency
{
    public Skill skill;

    public int requiredLevel;

    public bool MeetRequirement()
    {
        return skill.level >= requiredLevel;
    }

    public override string ToString()
    {
        return $"{skill.name} {requiredLevel}";
    }
}

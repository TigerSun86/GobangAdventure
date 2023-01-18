using System;
using System.Collections.Generic;

[Serializable]
public class SkillConfig
{
    public SkillId id;

    public string name;

    public string description;

    public List<SkillId> dependencies;

    public List<SkillLevelConfig> levels;
}

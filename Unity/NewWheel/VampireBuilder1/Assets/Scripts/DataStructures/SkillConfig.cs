using System;
using System.Collections.Generic;

[Serializable]
public class SkillConfig
{
    public string id;

    public string name;

    public string description;

    public List<string> dependencies;

    public List<SkillLevelConfig> levels;
}

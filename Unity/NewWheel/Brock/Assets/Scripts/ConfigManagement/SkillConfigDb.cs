using System.Collections.Generic;
using UnityEngine;

public class SkillConfigDb
{
    private Dictionary<string, SkillConfig> skillConfigMap;

    public SkillConfigDb(List<SkillConfig> skillConfigs)
    {
        this.skillConfigMap = new Dictionary<string, SkillConfig>();
        foreach (SkillConfig skill in skillConfigs)
        {
            string key = skill.skillName + skill.level;
            if (this.skillConfigMap.ContainsKey(key))
            {
                Debug.LogWarning($"Duplicate skill config found: {key}. Skipping.");
                continue;
            }

            this.skillConfigMap.Add(key, skill);
        }
    }

    public SkillConfig Get(string name, int level)
    {
        return Get(name + level);
    }

    public SkillConfig Get(string id)
    {
        if (skillConfigMap.TryGetValue(id, out SkillConfig skill))
        {
            return skill;
        }

        Debug.LogWarning($"Skill config not found for id: {id}");
        return null;
    }
}

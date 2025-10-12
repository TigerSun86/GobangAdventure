using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public class SkillConfigDb
{

    [SerializeField]
    private StringToSkillConfigDictionary skillConfigMap;

    public SkillConfigDb(List<SkillConfig> skillConfigs)
    {
        this.skillConfigMap = new StringToSkillConfigDictionary();
        foreach (SkillConfig skill in skillConfigs)
        {
            string key = skill.skillName + skill.level;
            if (this.skillConfigMap.ContainsKey(key))
            {
                Debug.LogWarning($"Duplicate skill config found: {key}. Skipping.");
                continue;
            }

            this.skillConfigMap.Add(key, skill);

            // JsonSerializerSettings settings = new JsonSerializerSettings
            // {
            //     Converters = { new StringEnumConverter() }
            // };
            // string json = JsonConvert.SerializeObject(skill, Formatting.Indented, settings);
            // System.IO.Directory.CreateDirectory("SerializedSkills");
            // System.IO.File.WriteAllText($"SerializedSkills\\{key}.json", json);
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

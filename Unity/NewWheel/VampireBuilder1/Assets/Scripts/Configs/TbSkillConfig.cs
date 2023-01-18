using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOneOfEach/TbSkillConfig")]
public class TbSkillConfig : ScriptableObject
{
    [SerializeField] StringSkillConfigDictionary skills;

    public void SetTbSkillConfig(List<SkillConfig> skills)
    {
        foreach (SkillConfig skillConfig in skills)
        {
            this.skills[skillConfig.id] = skillConfig;
        }
    }

    public SkillConfig GetSkillConfig(string id)
    {
        if (!skills.ContainsKey(id))
        {
            throw new ArgumentException($"Could not find a skill with id {id}");
        }

        return skills[id];
    }
}

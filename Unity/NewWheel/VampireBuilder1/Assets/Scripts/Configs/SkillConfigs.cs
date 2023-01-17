using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOneOfEach/SkillConfigs")]
public class SkillConfigs : ScriptableObject
{
    [SerializeField] List<SkillConfig> skills;

    public void SetSkillConfigs(List<SkillConfig> skills)
    {
        this.skills = skills;
    }

    public SkillConfig GetSkillConfig(string id)
    {
        SkillConfig skill = skills.FirstOrDefault(s => s.id == id);
        if (skill == null)
        {
            throw new ArgumentException($"Could not find a skill with id {id}");
        }

        return skill;
    }
}

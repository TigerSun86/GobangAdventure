using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOneOfEach/TbSkillConfig")]
public class TbSkillConfig : ScriptableObject
{
    [SerializeField] SkillIdToSkillConfigDictionary skills;

    public void SetTbSkillConfig(List<SkillConfig> skills)
    {
        this.skills.Clear();
        foreach (SkillConfig skillConfig in skills)
        {
            this.skills[skillConfig.id] = skillConfig;
        }
    }

    public SkillConfig GetSkillConfig(SkillId id)
    {
        if (!skills.ContainsKey(id))
        {
            throw new ArgumentException($"Could not find a skill with id {id}");
        }

        return skills[id];
    }
}

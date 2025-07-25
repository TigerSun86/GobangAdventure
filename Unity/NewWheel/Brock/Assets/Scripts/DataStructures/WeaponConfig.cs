using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponConfig
{
    public string weaponName;

    public int level;

    public WeaponBaseType weaponBaseType;

    public int price;

    public int health;

    public SkillConfig attackSkill;

    public SkillConfig skill1;

    public SkillConfig skill2;

    public Sprite sprite;

    public string GetId()
    {
        return $"{weaponName} {level}";
    }

    public SkillConfig[] GetSkills()
    {
        List<SkillConfig> skills = new List<SkillConfig>
        {
            attackSkill
        };

        if (!string.IsNullOrWhiteSpace(skill1.skillName))
        {
            skills.Add(skill1);
        }

        if (!string.IsNullOrWhiteSpace(skill2.skillName))
        {
            skills.Add(skill2);
        }

        return skills.ToArray();
    }
}
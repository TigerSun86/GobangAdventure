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

    public SkillConfig skill3;

    public int experienceWorth;

    public int experienceToNextLevel;

    public bool isPurchasable;

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
        else
        {
            skills.Add(null);
        }

        if (!string.IsNullOrWhiteSpace(skill2.skillName))
        {
            skills.Add(skill2);
        }
        else
        {
            skills.Add(null);
        }

        if (!string.IsNullOrWhiteSpace(skill3.skillName))
        {
            skills.Add(skill3);
        }
        else
        {
            skills.Add(null);
        }

        return skills.ToArray();
    }

    public bool IsNextLevelOf(WeaponConfig weaponConfig)
    {
        return this.weaponName == weaponConfig.weaponName
            && this.level == weaponConfig.level + 1;
    }
}
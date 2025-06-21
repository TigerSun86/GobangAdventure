using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponConfig2
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

        if (skill1 != null)
        {
            skills.Add(skill1);
        }

        if (skill2 != null)
        {
            skills.Add(skill2);
        }

        return skills.ToArray();
    }
}
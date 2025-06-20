using System;
using UnityEngine;

[Serializable]
public class WeaponConfig2
{
    public string name;

    public int level;

    public WeaponBaseType weaponBaseType;

    public int price;

    public int health;

    public SkillConfig attackSkill;

    public SkillConfig skill1;

    public SkillConfig skill2;

    public Sprite sprite;
}
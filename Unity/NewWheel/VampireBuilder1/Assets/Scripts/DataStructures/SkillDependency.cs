using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillDependency
{
    public SkillBase skill;

    public int level;

    public override string ToString()
    {
        return $"{skill.GetName()} {level}";
    }
}
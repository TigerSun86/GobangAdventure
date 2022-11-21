using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseAttackSkill : SkillBase
{
    public override string GetName()
    {
        return "Increase Attack";
    }

    public override string GetNextLevelDescription()
    {
        return "Increase the player attack by 1";
    }
}

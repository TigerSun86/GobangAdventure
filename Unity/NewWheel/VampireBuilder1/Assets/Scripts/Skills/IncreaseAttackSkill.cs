using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class IncreaseAttackSkill : SkillData
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

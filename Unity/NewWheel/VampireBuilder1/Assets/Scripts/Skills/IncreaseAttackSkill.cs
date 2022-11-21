using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseAttackSkill : SkillBase
{
    [SerializeField] FloatVariable attackFactor;

    public override void LevelUp()
    {
        base.LevelUp();
        attackFactor.ApplyChange(1);
    }

    public override string GetName()
    {
        return "Increase Attack";
    }

    public override string GetNextLevelDescription()
    {
        return "Increase the player attack by 1";
    }
}

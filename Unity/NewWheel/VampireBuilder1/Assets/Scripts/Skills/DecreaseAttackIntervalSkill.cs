using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecreaseAttackIntervalSkill : SkillBase
{
    [SerializeField] float changeAmount = 0.1f;

    [SerializeField] FloatVariable attackIntervalFactor;

    public override void LevelUp()
    {
        base.LevelUp();
        attackIntervalFactor.ApplyChange(-changeAmount);
    }

    public override string GetName()
    {
        return "Decrease Attack Interval";
    }

    public override string GetNextLevelDescription()
    {
        return "Decrease the player attack interval by " + changeAmount;
    }
}

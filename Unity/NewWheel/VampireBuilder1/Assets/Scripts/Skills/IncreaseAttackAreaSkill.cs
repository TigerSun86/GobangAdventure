using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseAttackAreaSkill : SkillBase
{
    [SerializeField] float changeAmount = 1f;

    [SerializeField] FloatVariable attackAreaFactor;

    public override void LevelUp()
    {
        base.LevelUp();
        attackAreaFactor.ApplyChange(changeAmount);
    }

    public override string GetName()
    {
        return "Increase Attack Area";
    }

    public override string GetNextLevelDescription()
    {
        return "Increase the player attack area by " + changeAmount;
    }
}

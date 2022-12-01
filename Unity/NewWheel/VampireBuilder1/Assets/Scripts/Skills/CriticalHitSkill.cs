using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalHitSkill : SkillBase
{
    [SerializeField] float changeAmount = 0.2f;

    [SerializeField] FloatVariable criticalHitChance;

    public override void LevelUp()
    {
        base.LevelUp();
        criticalHitChance.ApplyChange(changeAmount);
    }

    public override string GetName()
    {
        return "Critical Hit";
    }

    public override string GetNextLevelDescription()
    {
        return "Increase the critical hit chance by " + changeAmount;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMoveSpeedSkill : SkillBase
{
    [SerializeField] float changeAmount = 1f;

    [SerializeField] FloatVariable attackFactor;

    public override void LevelUp()
    {
        base.LevelUp();
        attackFactor.ApplyChange(changeAmount);
    }

    public override string GetName()
    {
        return "Increase Bullet Speed";
    }

    public override string GetNextLevelDescription()
    {
        return "Increase the bullet speed by " + changeAmount;
    }
}

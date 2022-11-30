using UnityEngine;

public class PierceSkill : SkillBase
{
    [SerializeField] int changeAmount = 1;

    [SerializeField] IntVariable pierceCount;

    public override void LevelUp()
    {
        base.LevelUp();
        pierceCount.ApplyChange(changeAmount);
    }

    public override string GetName()
    {
        return "Pierce";
    }

    public override string GetNextLevelDescription()
    {
        return $"Projectile can pass through {changeAmount} more enemies";
    }
}
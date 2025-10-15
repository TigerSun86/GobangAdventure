using System;
using System.Collections.Generic;

public class ActionBase
{
    protected WeaponSuit ownerWeaponSuit;

    protected SkillBase ownerSkill;

    public ActionBase(WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
    {
        this.ownerWeaponSuit = ownerWeaponSuit;
        this.ownerSkill = ownerSkill;
    }

    public void Apply()
    {
        foreach (WeaponSuit target in GetTargets())
        {
            Apply(target);
        }
    }

    protected virtual void Apply(WeaponSuit target)
    {
        throw new NotImplementedException();
    }

    protected virtual ActionConfig GetConfig()
    {
        throw new NotImplementedException();
    }

    private IEnumerable<WeaponSuit> GetTargets()
    {
        return GetConfig().actionTargetConfig.actionTargetType switch
        {
            ActionTargetType.CASTER => new WeaponSuit[] { this.ownerWeaponSuit },
            ActionTargetType.SKILL_SELECTED => this.ownerSkill.targets,
            _ => throw new System.NotSupportedException($"Action target type not supported: {GetConfig().actionTargetConfig.actionTargetType}")
        };
    }
}
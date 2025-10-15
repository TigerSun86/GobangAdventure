using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionBase
{
    protected WeaponSuit ownerWeaponSuit;

    protected SkillBase ownerSkill;

    public ActionBase(WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
    {
        this.ownerWeaponSuit = ownerWeaponSuit;
        this.ownerSkill = ownerSkill;
    }

    public void Apply(SkillEventContext skillEventContext)
    {
        foreach (WeaponSuit target in GetTargets(skillEventContext))
        {
            Apply(skillEventContext, target);
        }
    }

    protected virtual void Apply(SkillEventContext skillEventContext, WeaponSuit target)
    {
        throw new NotImplementedException();
    }

    protected virtual ActionConfig GetConfig()
    {
        throw new NotImplementedException();
    }

    private IEnumerable<WeaponSuit> GetTargets(SkillEventContext skillEventContext)
    {
        return GetConfig().actionTargetConfig.actionTargetType switch
        {
            ActionTargetType.CASTER => new WeaponSuit[] { this.ownerWeaponSuit },
            ActionTargetType.SKILL_SELECTED => this.ownerSkill.targets,
            ActionTargetType.ATTACKER => skillEventContext.damageData == null
            ? LogDamageDataNullAndReturnEmpty()
            : new WeaponSuit[] { skillEventContext.damageData.source.GetComponent<WeaponSuit>() },
            _ => throw new System.NotSupportedException($"Action target type not supported: {GetConfig().actionTargetConfig.actionTargetType}")
        };

        IEnumerable<WeaponSuit> LogDamageDataNullAndReturnEmpty()
        {
            Debug.LogError("skillEventContext.damageData is null in ActionBase.GetTargets for ATTACKER target type.");
            return Array.Empty<WeaponSuit>();
        }
    }
}
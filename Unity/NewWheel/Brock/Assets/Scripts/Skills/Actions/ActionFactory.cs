using System;

public static class ActionFactory
{
    public static ActionBase Create(ActionConfig config, WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
    {
        return config switch
        {
            DamageActionConfig c => new DamageAction(c, ownerWeaponSuit, ownerSkill),
            ApplyAuraModifierActionConfig c => new ApplyAuraModifierAction(c, ownerWeaponSuit, ownerSkill),
            ApplyModifierActionConfig c => new ApplyModifierAction(c, ownerWeaponSuit, ownerSkill),
            LifestealActionConfig c => new LifestealAction(c, ownerWeaponSuit, ownerSkill),
            StunActionConfig c => new StunAction(c, ownerWeaponSuit, ownerSkill),
            _ => throw new NotSupportedException($"Action not supported: {config.type}")
        };
    }
}
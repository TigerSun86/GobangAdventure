public class RemoveModifierAction : ActionBase
{
    private RemoveModifierActionConfig config;

    public RemoveModifierAction(RemoveModifierActionConfig config, WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
        : base(ownerWeaponSuit, ownerSkill)
    {
        this.config = config;
    }

    protected override void Apply(SkillEventContext skillEventContext, WeaponSuit target)
    {
        target.modifierContainer.RemoveModifier(this.config.modifierConfig.id);
    }

    protected override ActionConfig GetConfig()
    {
        return this.config;
    }
}
public class ApplyModifierAction : ActionBase
{
    private ApplyModifierActionConfig config;

    public ApplyModifierAction(ApplyModifierActionConfig config, WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
        : base(ownerWeaponSuit, ownerSkill)
    {
        this.config = config;
    }

    protected override void Apply(WeaponSuit target)
    {
        target.modifierContainer.AddModifier(new Modifier(this.config.modifierConfig.Clone()));
    }

    protected override ActionConfig GetConfig()
    {
        return this.config;
    }
}
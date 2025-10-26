public class ReviveAction : ActionBase
{
    private ReviveActionConfig config;

    public ReviveAction(ReviveActionConfig config, WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
        : base(ownerWeaponSuit, ownerSkill)
    {
        this.config = config;
    }

    protected override void Apply(SkillEventContext skillEventContext, WeaponSuit target)
    {
        Health health = target.weaponStand.GetComponent<Health>();
        Healable healable = target.weaponStand.GetComponent<Healable>();
        healable.TakeHealing(target.gameObject, this.ownerSkill.skillConfig.skillType, health.maxHealth);
    }

    protected override ActionConfig GetConfig()
    {
        return this.config;
    }
}
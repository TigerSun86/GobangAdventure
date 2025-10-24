using Unity.VisualScripting;

public class HealAction : ActionBase
{
    private HealActionConfig config;

    public HealAction(HealActionConfig config, WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
        : base(ownerWeaponSuit, ownerSkill)
    {
        this.config = config;
    }

    protected override void Apply(SkillEventContext skillEventContext, WeaponSuit target)
    {
        Heal(target);
    }

    protected override ActionConfig GetConfig()
    {
        return this.config;
    }

    private void Heal(WeaponSuit target)
    {
        if (target.IsDestroyed())
        {
            return;
        }

        Healable healable = target.weaponStand.GetComponent<Healable>();
        healable.TakeHealing(this.ownerWeaponSuit.gameObject, this.ownerSkill.skillConfig.skillType, (int)this.config.amount);
    }
}
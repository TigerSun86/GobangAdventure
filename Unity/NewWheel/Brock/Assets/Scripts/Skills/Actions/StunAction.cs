using System.Collections.Generic;

public class StunAction : ActionBase
{
    private static readonly ModifierConfig MODIFIER_CONFIG_TEMPLATE = new ModifierConfig
    {
        buffType = BuffType.Stun,
        buffIcon = "Sprites/Stun",
        buffIconSprite = ParserUtility.ParseSpriteSafe("Sprites/Stun", "buffIcon"),
        states = new Dictionary<ModifierStateType, ModifierStateValue>()
        {
            {ModifierStateType.MODIFIER_STATE_STUNNED, ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED}
        }
    };

    private StunActionConfig config;

    public StunAction(StunActionConfig config, WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
        : base(ownerWeaponSuit, ownerSkill)
    {
        this.config = config;
    }

    protected override void Apply(SkillEventContext skillEventContext, WeaponSuit target)
    {
        ModifierConfig modifierConfig = StunAction.MODIFIER_CONFIG_TEMPLATE.Clone();
        modifierConfig.duration = this.config.duration;

        target.modifierContainer.AddModifier(new Modifier(modifierConfig, this.ownerSkill));
    }

    protected override ActionConfig GetConfig()
    {
        return this.config;
    }
}

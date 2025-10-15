using UnityEngine;

public class LifestealAction : ActionBase
{
    private LifestealActionConfig config;

    public LifestealAction(LifestealActionConfig config, WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
        : base(ownerWeaponSuit, ownerSkill)
    {
        this.config = config;
    }

    protected override void Apply(SkillEventContext skillEventContext, WeaponSuit target)
    {
        DamageData damageData = skillEventContext.damageData;
        if (damageData == null)
        {
            Debug.LogError("DamageData is null.");
            return;
        }

        if (damageData.damageType == DamageType.NONE
            || (damageData.damageType & DamageType.HEALING) != 0)
        {
            Debug.LogError($"Invalid damage type {damageData.damageType}.");
            return;
        }

        if (damageData.actualAmount <= 0)
        {
            return;
        }

        if (this.config.lifestealPercentage > 0)
        {
            int healAmount = (int)(damageData.actualAmount * this.config.lifestealPercentage);
            if (healAmount < 1)
            {
                // Ensure at least 1 HP is healed.
                healAmount = 1;
            }

            // Note: healing source and target are the same.
            Healable healable = target.GetComponentInChildren<Healable>();
            // TODO: Change the skill type to the skill triggering the buff.
            healable.TakeHealing(target.gameObject, damageData.skillType, healAmount);
            Debug.LogError($"Healed {target.weaponConfig.GetId()} {healAmount}.");
        }
    }

    protected override ActionConfig GetConfig()
    {
        return this.config;
    }
}
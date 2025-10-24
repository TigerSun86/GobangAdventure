using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class DamageAction : ActionBase
{
    private DamageActionConfig config;

    public DamageAction(DamageActionConfig config, WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
        : base(ownerWeaponSuit, ownerSkill)
    {
        this.config = config;
    }

    protected override void Apply(SkillEventContext skillEventContext, WeaponSuit target)
    {
        DealDamage(target);
    }

    protected override ActionConfig GetConfig()
    {
        return this.config;
    }

    private void DealDamage(WeaponSuit target)
    {
        if (target.IsDestroyed())
        {
            return;
        }

        double damage = this.config.amount;

        damage = CalculateAttackIncrease(damage);
        DamageType damageType = DamageType.NORMAL_ATTACK;
        WeaponBaseTypeMatchResult matchResult = WeaponBaseTypeUtility.GetMatchResult(
            this.ownerWeaponSuit.weaponBaseType,
            target.weaponBaseType);
        if (matchResult == WeaponBaseTypeMatchResult.STRONG)
        {
            damageType = DamageType.STRONG_ATTACK;
            damage *= 2;
        }
        else if (matchResult == WeaponBaseTypeMatchResult.WEAK)
        {
            damageType = DamageType.WEAK_ATTACK;
            damage /= 2;
        }

        Damagable damagable = target.weaponStand.GetComponent<Damagable>();
        damagable.TakeDamage(this.ownerWeaponSuit.gameObject, this.ownerSkill.skillConfig.skillType, (int)damage, damageType);
    }

    private double CalculateAttackIncrease(double baseDamage)
    {
        double damageDelta = this.ownerWeaponSuit.GetComponent<BuffTracker>().Get(BuffType.AttackAmountChange)
            .Sum(b => b.value1);
        return baseDamage + damageDelta;
    }
}
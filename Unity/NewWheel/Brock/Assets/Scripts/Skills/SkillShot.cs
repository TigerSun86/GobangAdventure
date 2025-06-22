using Unity.VisualScripting;

public class SkillShot : SkillBase
{
    public SkillShot(WeaponSuit weaponSuit, SkillConfig skillConfig) : base(weaponSuit, skillConfig)
    {
    }

    // Returns true if finished.
    protected override bool Act()
    {
        if (!AreTargetsValid())
        {
            return true;
        }

        float remainingTime = skillConfig.actionTime - timeInCurrentState;
        if (remainingTime <= 0)
        {
            foreach (WeaponSuit target in targets)
            {
                DealDamage(target);
            }
            return true;
        }
        return false;
    }

    // Returns true if finished.
    protected override bool Recover()
    {
        float remainingTime = skillConfig.recoveryTime - timeInCurrentState;
        return remainingTime <= 0;
    }

    private void DealDamage(WeaponSuit target)
    {
        if (target.IsDestroyed())
        {
            return;
        }

        double damage = this.skillConfig.value;
        DamageType damageType = DamageType.NORMAL_ATTACK;
        Damagable damagable = target.weaponStand.GetComponent<Damagable>();
        damagable.TakeDamage((int)damage, damageType);
    }
}
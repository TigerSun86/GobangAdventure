using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SkillBlinkAttack : SkillBase
{
    private BuffTracker buffTracker;

    public override void Initialize(WeaponSuit weaponSuit, SkillConfig skillConfig, int skillIndex)
    {
        base.Initialize(weaponSuit, skillConfig, skillIndex);
        this.buffTracker = this.weaponSuit.GetComponent<BuffTracker>();
        if (this.buffTracker == null)
        {
            Debug.LogError("WeaponSuit does not have a BuffTracker component.");
        }
    }

    // Returns true if finished.
    // Disappears, then move to target, then reappears and attacks.
    protected override bool Act()
    {
        if (!AreTargetsValid())
        {
            return true;
        }

        this.weaponSuit.weaponItem.SetVisibility(false);

        float remainingTime = skillConfig.actionTime - timeInCurrentState;
        this.weaponSuit.weaponItem.MoveToTarget(targets[0].transform, remainingTime);
        if (remainingTime <= 0)
        {
            this.weaponSuit.weaponItem.SetVisibility(true);
            foreach (WeaponSuit target in targets)
            {
                DealDamage(target);
            }

            return true;
        }

        return false;
    }

    // Returns true if finished.
    // Stay at the target position until recovery time is up, then return to stand.
    protected override bool Recover()
    {
        float remainingTime = skillConfig.recoveryTime - timeInCurrentState;
        if (remainingTime <= 0)
        {
            // Note: this is a no op.
            weaponSuit.weaponItem.ReturnToStand(0);
            return true;
        }

        return false;
    }

    private void DealDamage(WeaponSuit target)
    {
        if (target.IsDestroyed())
        {
            return;
        }

        double damage = this.skillConfig.value + StatsManager.Instance.attack;
        damage = CalculateAttackIncrease(damage);
        DamageType damageType = DamageType.NORMAL_ATTACK;
        WeaponBaseTypeMatchResult matchResult = WeaponBaseTypeUtility.GetMatchResult(
            weaponSuit.weaponBaseType,
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
        damagable.TakeDamage(this.weaponSuit.gameObject, this.skillConfig.skillType, (int)damage, damageType);
    }

    private double CalculateAttackIncrease(double baseDamage)
    {
        double damageDelta = this.buffTracker.Get(BuffType.AttackAmountChange)
            .Sum(b => b.value1);
        return baseDamage + damageDelta;
    }
}
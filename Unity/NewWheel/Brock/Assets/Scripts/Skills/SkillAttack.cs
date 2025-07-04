using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SkillAttack : SkillBase
{
    private BuffTracker buffTracker;

    public override void Initialize(WeaponSuit weaponSuit, SkillConfig skillConfig)
    {
        base.Initialize(weaponSuit, skillConfig);
        this.buffTracker = this.weaponSuit.GetComponent<BuffTracker>();
        if (this.buffTracker == null)
        {
            Debug.LogError("WeaponSuit does not have a BuffTracker component.");
        }
    }

    // Returns true if finished.
    protected override bool Act()
    {
        if (!AreTargetsValid())
        {
            return true;
        }

        float remainingTime = skillConfig.actionTime - timeInCurrentState;
        weaponSuit.weaponItem.MoveToTarget(targets[0].transform, remainingTime);
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
        weaponSuit.weaponItem.ReturnToStand(remainingTime);

        return remainingTime <= 0;
    }

    private void DealDamage(WeaponSuit target)
    {
        if (target.IsDestroyed())
        {
            return;
        }

        double damage = this.skillConfig.value;
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

        Buff criticalHit = CalculateCriticalHit();
        if (criticalHit != null)
        {
            damage *= criticalHit.value2;
            damageType |= DamageType.CRITICAL_HIT;
        }

        Damagable damagable = target.weaponStand.GetComponent<Damagable>();
        damagable.TakeDamage((int)damage, damageType);
    }

    private double CalculateAttackIncrease(double baseDamage)
    {
        double damageDelta = this.buffTracker.Get(BuffType.AttackAmountChange)
            .Sum(b => b.value1);
        return baseDamage + damageDelta;
    }

    private Buff CalculateCriticalHit()
    {
        IEnumerable<Buff> criticalBuffs = this.buffTracker.Get(BuffType.CriticalHit)
            .OrderByDescending(b => b.value2) // Sort by value2 (critical hit multiplier).
            .Where(b => b.value1 > UnityEngine.Random.value); // If true, then this is a critical hit.
        return criticalBuffs.FirstOrDefault();
    }
}
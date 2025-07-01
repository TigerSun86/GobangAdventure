using UnityEngine;

public class SkillCriticalHit : SkillBase
{
    public override void Initialize(WeaponSuit weaponSuit, SkillConfig skillConfig)
    {
        base.Initialize(weaponSuit, skillConfig);
        BuffTracker buffTracker = this.weaponSuit.GetComponent<BuffTracker>();
        if (buffTracker == null)
        {
            Debug.LogError("WeaponSuit does not have a BuffTracker component.");
            return;
        }

        buffTracker.Add(new Buff
        {
            buffType = BuffType.CriticalHit,
            duration = this.skillConfig.buff1.duration,
            value1 = this.skillConfig.buff1.value1,
            value2 = this.skillConfig.buff1.value2,
        });
    }
}
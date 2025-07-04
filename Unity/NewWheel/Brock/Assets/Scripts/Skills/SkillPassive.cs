public class SkillPassive : SkillBase
{
    public override void Initialize(WeaponSuit weaponSuit, SkillConfig skillConfig)
    {
        base.Initialize(weaponSuit, skillConfig);
        if (SelectTarget())
        {
            foreach (WeaponSuit target in this.targets)
            {
                BuffTracker buffTracker = target.GetComponent<BuffTracker>();
                if (buffTracker == null)
                {
                    continue;
                }

                Buff buffClone = this.skillConfig.buff1.Clone();
                buffTracker.Add(buffClone);
            }
        }
    }
}
public class SkillPassive : SkillBase
{
    public override void Initialize(WeaponSuit weaponSuit, SkillConfig skillConfig, int skillIndex)
    {
        base.Initialize(weaponSuit, skillConfig, skillIndex);
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

                if (this.skillConfig.buff2 != null && this.skillConfig.buff2.buffType != BuffType.None)
                {
                    Buff buffClone2 = this.skillConfig.buff2.Clone();
                    buffTracker.Add(buffClone2);
                }
            }
        }
    }
}
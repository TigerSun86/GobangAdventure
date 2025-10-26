using System.Linq;

public class SkillBuffBlockingCd : SkillBase
{
    // Returns true if finished.
    protected override bool Act()
    {
        if (!AreTargetsValid())
        {
            return true;
        }

        // foreach (WeaponSuit target in this.targets)
        // {
        //     BuffTracker buffTracker = target.GetComponent<BuffTracker>();
        //     if (buffTracker == null)
        //     {
        //         continue;
        //     }

        //     Buff buffClone = this.skillConfig.buff1.Clone();
        //     buffTracker.Add(buffClone);

        //     if (this.skillConfig.buff2 != null && this.skillConfig.buff2.buffType != BuffType.None)
        //     {
        //         Buff buffClone2 = this.skillConfig.buff2.Clone();
        //         buffTracker.Add(buffClone2);
        //     }
        // }

        return true;
    }

    // Returns true if finished.
    protected override bool Recover()
    {
        return true;
    }

    protected override float GetCalculatedCdTime()
    {
        // if (IsBuffOnTargets())
        // {
        //     return float.PositiveInfinity;
        // }

        return base.GetCalculatedCdTime();
    }

    protected override bool EnableUpdateTimeInCurrentState()
    {
        // Stop proceeding CD time if buffs haven't been consumed.
        return base.EnableUpdateTimeInCurrentState();
    }

    private bool IsBuffOnTargets()
    {
        if (AreTargetsValid())
        {
            foreach (WeaponSuit target in this.targets)
            {
                BuffTracker buffTracker = target.GetComponent<BuffTracker>();
                if (buffTracker == null)
                {
                    continue;
                }

                // TODO: Check if the buff came from this skill.
                Buff buff = buffTracker.Get(this.skillConfig.buff1.buffType)
                    .FirstOrDefault();
                if (buff != null)
                {
                    return true;
                }

                if (this.skillConfig.buff2 != null && this.skillConfig.buff2.buffType != BuffType.None)
                {
                    Buff buff2 = buffTracker.Get(this.skillConfig.buff2.buffType)
                        .FirstOrDefault();
                    if (buff2 != null)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
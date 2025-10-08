using System.Linq;
using UnityEngine;

public class ReviveWhenFaintingOnFaintingHandler : MonoBehaviour
{
    public void Handle(WeaponSuit weaponSuit)
    {
        BuffTracker buffTracker = weaponSuit.GetComponent<BuffTracker>();
        Buff ReviveWhenFaintingBuff = buffTracker.Get(BuffType.ReviveWhenFainting)
            .FirstOrDefault();
        if (ReviveWhenFaintingBuff != null)
        {
            Health health = weaponSuit.weaponStand.GetComponent<Health>();
            Healable healable = weaponSuit.weaponStand.GetComponent<Healable>();
            // TODO: Change the skill type to the skill triggering the buff.
            healable.TakeHealing(weaponSuit.gameObject, SkillType.Revive, health.maxHealth);

            // Remove the ReviveWhenFainting buff and the associated HealthLock buff.
            buffTracker.Remove(ReviveWhenFaintingBuff);
            Buff healthLockBuff = buffTracker.Get(BuffType.HealthLock)
                .FirstOrDefault();
            if (healthLockBuff != null)
            {
                buffTracker.Remove(healthLockBuff);
            }
            else
            {
                Debug.LogError("ReviveWhenFainting buff found without a corresponding HealthLock buff.");
            }
        }
    }
}